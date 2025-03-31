#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using nadena.dev.ndmf;
using nadena.dev.ndmf.preview;
using UnityEngine;

namespace lilToonNDMFUtility
{
    internal class LNUPreviewGroupingContext : ILNUContext
    {
        ComputeContext _context;
        GameObject _avatarRoot;
        Renderer[]? _renderers;


        public LNUPreviewGroupingContext(ComputeContext context, GameObject avatarRoot)
        {
            _context = context;
            _avatarRoot = avatarRoot;
        }

        public IEnumerable<Renderer> EnumerateRenderer()
        {
            return _renderers ??= _context.GetComponentsInChildren<Renderer>(_avatarRoot, true).Where(r => r is SkinnedMeshRenderer or MeshRenderer).ToArray();
        }
        public IEnumerable<Material?> GetRenderersMaterial(Renderer renderer)
        {
            return LookAtGet(renderer, GetShardMaterial, (l, r) => l.SequenceEqual(r));
            Material?[] GetShardMaterial(Renderer r) { return renderer.sharedMaterials; }
        }
        public void GetMutableMaterial(ref Material material) { throw new NotImplementedException(); }
        public bool OriginEqual(UnityEngine.Object l, UnityEngine.Object r) { return ObjectRegistry.GetReference(l) == ObjectRegistry.GetReference(r); }
        public T[] GetComponentsInChildren<T>(GameObject gameObject, bool includeInactive = false)
        { return _context.GetComponentsInChildren(gameObject, typeof(T), includeInactive).Cast<T>().ToArray(); }
        public void LookAt(UnityEngine.Object obj) { _context.Observe(obj); }
        public TOut LookAtGet<TObj, TOut>(TObj obj, Func<TObj, TOut> getAction, Func<TOut, TOut, bool>? comp = null)
        where TObj : UnityEngine.Object
        {
            return _context.Observe(obj, getAction, comp);
        }
        public void LoadTexture2D(RenderTexture write, Texture2D load) { throw new NotImplementedException(); }
        public void RegisterReadBack2Replace(Texture originTexture, RenderTexture rt) { throw new NotImplementedException(); }
    }
    internal class LNUPreviewContext : ILNUContext, IDisposable
    {
        ComputeContext _context;
        IEnumerable<Renderer> _proxies;
        Dictionary<Material, Material> _origin2MutableMaterials = new();
        HashSet<RenderTexture> _registeredRenderTexture = new();


        public LNUPreviewContext(ComputeContext context, IEnumerable<Renderer> proxies)
        {
            _context = context;
            _proxies = proxies;
        }

        public IEnumerable<Renderer> EnumerateRenderer() { return _proxies; }
        public IEnumerable<Material?> GetRenderersMaterial(Renderer renderer)
        {
            return LookAtGet(renderer, GetShardMaterial, (l, r) => l.SequenceEqual(r));
            Material?[] GetShardMaterial(Renderer r) { return renderer.sharedMaterials; }
        }
        public void GetMutableMaterial(ref Material material)
        {
            if (_origin2MutableMaterials.ContainsValue(material)) { return; }

            var originMaterial = material;
            var mutableMaterial = material = UnityEngine.Object.Instantiate(material);
            mutableMaterial.name = originMaterial.name + "(LNU Clone)";

            _origin2MutableMaterials[originMaterial] = mutableMaterial;
            ObjectRegistry.RegisterReplacedObject(originMaterial, mutableMaterial);

            foreach (var r in EnumerateRenderer())
                r.sharedMaterials = r.sharedMaterials.Select(i => i == originMaterial ? mutableMaterial : i).ToArray();
        }
        public bool OriginEqual(UnityEngine.Object l, UnityEngine.Object r) { return ObjectRegistry.GetReference(l) == ObjectRegistry.GetReference(r); }
        public T[] GetComponentsInChildren<T>(GameObject gameObject, bool includeInactive = false)
        { return _context.GetComponentsInChildren(gameObject, typeof(T), includeInactive).Cast<T>().ToArray(); }

        public void LookAt(UnityEngine.Object obj) { _context.Observe(obj); }
        public TOut LookAtGet<TObj, TOut>(TObj obj, Func<TObj, TOut> getAction, Func<TOut, TOut, bool>? comp = null)
        where TObj : UnityEngine.Object
        {
            return _context.Observe(obj, getAction, comp);
        }

        public void ReplaceRecall(Renderer r)
        {
            r.sharedMaterials = r.sharedMaterials.Select(i => _origin2MutableMaterials.TryGetValue(i, out var rm) ? rm : i).ToArray();
        }

        public void Dispose()
        {
            foreach (var m in _origin2MutableMaterials.Values) { UnityEngine.Object.DestroyImmediate(m); }
            _origin2MutableMaterials.Clear();

            foreach (var r in _registeredRenderTexture) { LNUTempRt.Rel(r); }
            _registeredRenderTexture.Clear();
        }

        public void LoadTexture2D(RenderTexture write, Texture2D load) { Graphics.Blit(load, write); }
        public void RegisterReadBack2Replace(Texture originTexture, RenderTexture rt)
        {
            _registeredRenderTexture.Add(rt);
            if (originTexture != null) ObjectRegistry.RegisterReplacedObject(originTexture, rt);
        }
    }
}
