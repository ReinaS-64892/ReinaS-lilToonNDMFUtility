#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using nadena.dev.ndmf;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace lilToonNDMFUtility
{
    internal class LNUBuildContext : ILNUContext
    {
        BuildContext _context;
        Renderer[]? _renderers;
        Dictionary<RenderTexture, Texture2D?> _renderTextureOriginDict = new();
        Dictionary<Texture2D, Texture2D?> _needCompress2OriginTexture = new();

        // これを使うような実装は Preview が不可能になる。
        public BuildContext NDMFBuildContext => _context;

        public LNUBuildContext(BuildContext context)
        {
            _context = context;
        }

        public IEnumerable<Renderer> EnumerateRenderer()
        {
            return _renderers ??= _context.AvatarRootObject.GetComponentsInChildren<Renderer>(true);
        }
        public IEnumerable<Material> GetRenderersMaterial(Renderer renderer)
        {
            return renderer.sharedMaterials.Concat(Array.Empty<Material>());// TODO : animation parsing
        }


        public void GetMutableMaterial(ref Material material)
        {
            if (_context.AssetSaver.IsTemporaryAsset(material)) { return; }

            var originMaterial = material;
            var mutableMaterial = material = UnityEngine.Object.Instantiate(material);
            mutableMaterial.name = originMaterial.name + "(LNU Clone)";

            _context.AssetSaver.SaveAsset(mutableMaterial);
            ObjectRegistry.RegisterReplacedObject(originMaterial, mutableMaterial);

            foreach (var r in EnumerateRenderer())
                r.sharedMaterials = r.sharedMaterials.Select(i => i == originMaterial ? mutableMaterial : i).ToArray();

            // TODO : animation editing
        }
        public bool OriginEqual(UnityEngine.Object l, UnityEngine.Object r) { return ObjectRegistry.GetReference(l) == ObjectRegistry.GetReference(r); }
        public T[] GetComponentsInChildren<T>(GameObject gameObject, bool includeInactive = false)
        { return gameObject.GetComponentsInChildren<T>(includeInactive); }

        public void LoadTexture2D(RenderTexture write, Texture2D load)
        {
            // TODO : original load
            Graphics.Blit(load, write);
        }

        public void RegisterReadBack2Replace(Texture argOriginTexture, RenderTexture rt)
        {
            Texture? originTex = argOriginTexture;
            while (originTex is RenderTexture oRt && _renderTextureOriginDict.ContainsKey(oRt))
                originTex = _renderTextureOriginDict[oRt];

            _renderTextureOriginDict[rt] = originTex as Texture2D;
        }



        public void ReadBack()
        {
            var mats = this.GetContextAllMaterials();
            var readBackTextures = new Dictionary<RenderTexture, Texture2D>();
            foreach (var kv in _renderTextureOriginDict)
            {
                readBackTextures[kv.Key] = TextureUtil.CopyTexture2D(kv.Key, null, (kv.Value?.mipmapCount ?? 2) > 1);
            }
            TextureUtil.ReplaceTextureAll(this, mats, readBackTextures);
            foreach (var i in readBackTextures)
            {
                _needCompress2OriginTexture[readBackTextures[i.Key]] = _renderTextureOriginDict[i.Key];

                _context.AssetSaver.SaveAsset(i.Value);
            }
            _renderTextureOriginDict.Clear();
        }
        public void Compression()
        {
            TextureUtil.Compress(this, _needCompress2OriginTexture);
            _needCompress2OriginTexture.Clear();
        }

    }

    internal static class TextureUtil
    {

        // this code from TexTransTool
        // https://github.com/ReinaS-64892/TexTransTool/blob/50c97a92347b813c981c904b3c79a23f0a203be3/Runtime/Utils/TextureUtility.cs#L22-L58
        public static Texture2D CopyTexture2D(RenderTexture rt, TextureFormat? overrideFormat = null, bool? overrideUseMip = null)
        {
            var useMip = overrideUseMip ?? rt.useMipMap;
            var format = overrideFormat ?? GraphicsFormatUtility.GetTextureFormat(rt.graphicsFormat);
            var readMapCount = rt.useMipMap && useMip ? rt.mipmapCount : 1;

            Span<AsyncGPUReadbackRequest> asyncGPUReadbackRequests = stackalloc AsyncGPUReadbackRequest[readMapCount];
            for (var i = 0; readMapCount > i; i += 1)
            {
                asyncGPUReadbackRequests[i] = AsyncGPUReadback.Request(rt, i);
            }


            var texture = new Texture2D(rt.width, rt.height, format, useMip, !rt.sRGB);
            texture.name = rt.name + "_CopyTex2D";

            if (rt.useMipMap && useMip)
            {
                for (var layer = 0; readMapCount > layer; layer += 1)
                {
                    asyncGPUReadbackRequests[layer].WaitForCompletion();
                    using (var data = asyncGPUReadbackRequests[layer].GetData<Color32>())
                    {
                        texture.SetPixelData(data, layer);
                    }
                }
                texture.Apply(false);
            }
            else
            {
                asyncGPUReadbackRequests[0].WaitForCompletion();
                using (var data = asyncGPUReadbackRequests[0].GetData<Color32>())
                {
                    texture.SetPixelData(data, 0);
                }
                texture.Apply(true);
            }



            return texture;
        }
#nullable disable
        // this code from TexTransTool
        // https://github.com/ReinaS-64892/TexTransTool/blob/332190eeca460e0053619a8f50b979b31357a0cd/Runtime/Utils/MaterialUtility.cs
        public static Dictionary<Material, Material> ReplaceTextureAll<TexDist, TexNew>(ILNUContext ctx, IEnumerable<Material> materials, Dictionary<TexDist, TexNew> texturePair)
        where TexDist : Texture
        where TexNew : Texture
        {
            var outPut = new Dictionary<Material, Material>();
            foreach (var mat in materials)
            {
                var textures = GetAllTextureWithDictionary<TexDist>(mat);

                bool replacedFlag = false;
                foreach (var tex in textures) { if (texturePair.ContainsKey(tex.Value)) { replacedFlag = true; break; } }
                if (replacedFlag == false) { continue; }

                var mutMat = mat;
                ctx.GetMutableMaterial(ref mutMat);
                foreach (var tex in textures) { if (texturePair.TryGetValue(tex.Value, out var swapTex)) { mutMat.SetTexture(tex.Key, swapTex); } }
            }
            return outPut;
        }
        public static Dictionary<string, Tex> GetAllTextureWithDictionary<Tex>(this Material material) where Tex : Texture
        {
            var output = new Dictionary<string, Tex>();
            if (material == null || material.shader == null) { return output; }
            var shader = material.shader;
            var propCount = shader.GetPropertyCount();
            for (var i = 0; propCount > i; i += 1)
            {
                if (shader.GetPropertyType(i) != UnityEngine.Rendering.ShaderPropertyType.Texture) { continue; }
                var propName = shader.GetPropertyName(i);
                var texture = material.GetTexture(propName) as Tex;
                if (texture != null) { output.TryAdd(propName, texture); }
            }
            return output;
        }
#nullable enable

        public static IEnumerable<Tex> GetAllTex<Tex>(IEnumerable<Material> mats)
        where Tex : Texture
        {
            return mats.SelectMany(m => m.GetAllTextureWithDictionary<Tex>().Values).Distinct();
        }
        // this code from TexTransTool
        // https://github.com/ReinaS-64892/TexTransTool/blob/332190eeca460e0053619a8f50b979b31357a0cd/Editor/Domain/TextureManager.cs#L147-L198
        public static void Compress(ILNUContext ctx, Dictionary<Texture2D, Texture2D?> needCompress2OriginTexture)
        {
            var compressKV = needCompress2OriginTexture.Where(i => i.Key != null).ToDictionary(i => i.Key, i => i.Value);// Unity が勝手にテクスチャを破棄してくる場合があるので Null が入ってないか確認する必要がある。


            var targetTextures = GetAllTex<Texture2D>(ctx.GetContextAllMaterials())
                .Where(t => t != null)
                .Distinct()
                .Select(t => (t, compressKV.FirstOrDefault(kv => ctx.OriginEqual(kv.Key, t))))
                .Where(kvp => kvp.Item2.Key is not null && kvp.Item2.Value is not null)
                .Select(kvp => (kvp.t, kvp.Item2.Value))
                .Where(kv => GraphicsFormatUtility.IsCompressedFormat(kv.t.format) is false)
                .ToArray();

            // ほかツールが増やした場合のために自分が情報を持っているやつから派生した場合にフォールバック設定で圧縮が行われる
            foreach (var (tex, originTexture) in targetTextures)
            {
                if (originTexture == null) { continue; }
                EditorUtility.CompressTexture(tex, originTexture.format, TextureCompressionQuality.Best);
            }

            foreach (var tex in targetTextures) tex.t.Apply(false, true);
            foreach (var tex in targetTextures)
            {
                var sTexture = new SerializedObject(tex.t);

                var sStreamingMipmaps = sTexture.FindProperty("m_StreamingMipmaps");
                sStreamingMipmaps.boolValue = true;

                sTexture.ApplyModifiedPropertiesWithoutUndo();
            }
        }
    }
}
