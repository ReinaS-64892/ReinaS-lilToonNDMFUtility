#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace lilToonNDMFUtility
{
    internal interface ILNUContext
    {
        IEnumerable<Renderer> EnumerateRenderer();

        public IEnumerable<Material?> GetRenderersMaterial(Renderer renderer);
        void GetMutableMaterial(ref Material material);


        void LoadTexture2D(RenderTexture write, Texture2D load);

        // RenderTexture をマテリアルに書き込んでおいて、 originalTexture は ReadBackした後のフォーマットの決定などに使用する。
        void RegisterReadBack2Replace(Texture originTexture, RenderTexture rt);


        bool OriginEqual(UnityEngine.Object l, UnityEngine.Object r);


        void LookAt(UnityEngine.Object obj) { }
        TOut LookAtGet<TObj, TOut>(TObj obj, Func<TObj, TOut> getAction, Func<TOut, TOut, bool>? comp = null)
        where TObj : UnityEngine.Object
        {
            return getAction(obj);
        }
        T[] GetComponentsInChildren<T>(GameObject gameObject, bool includeInactive = false);
    }
    public delegate bool OriginEqual(UnityEngine.Object l, UnityEngine.Object r);

    internal static class LNUCtxUtil
    {
        public static IEnumerable<Material> GetCtxMaterials<T>(this ILNUContext ctx, T component)
        where T : Component, ITargetMaterialRange
        {
            switch (ctx.LookAtGet(component, c => c.TargetMaterialRange.RangeMode))
            {
                default:
                case MaterialRange.Mode.All:
                    {
                        return GetContextAllMaterials(ctx);
                    }
                case MaterialRange.Mode.ParentObject:
                    {
                        var sourceRenderers = ctx.LookAtGet(component, c => c.TargetMaterialRange.ParentObjects, (l, r) => l.SequenceEqual(r))
                            .Where(i => i != null)
                            .Select(i => i!.Get(component))
                            .SelectMany(g => ctx.GetComponentsInChildren<Renderer>(g, true))
                            .ToArray();
                        return ctx.GetCtxRenderers(sourceRenderers)
                            .SelectMany(r => ctx.GetRenderersMaterial(r))
                            .Where(i => i != null)
                            .Cast<Material>()
                            .Distinct();
                    }
                case MaterialRange.Mode.Manual:
                    { return ctx.GetCtxMaterials(ctx.LookAtGet(component, c => c.TargetMaterialRange.ManualSelections)); }
            }
        }

        public static IEnumerable<Material> GetContextAllMaterials(this ILNUContext ctx)
        {
            return ctx.EnumerateRenderer()
                .SelectMany(i => ctx.GetRenderersMaterial(i))
                .Where(i => i != null)
                .Cast<Material>()
                .Distinct();
        }

        public static IEnumerable<Material> GetCtxMaterials(this ILNUContext ctx, IEnumerable<Material?> sourceMaterials)
        {
            return ctx.EnumerateRenderer()
                .SelectMany(r => ctx.GetRenderersMaterial(r))
                .Where(i => i != null)
                .Cast<Material>()
                .Distinct()
                .Where(m => sourceMaterials.Any(s => s != null ? ctx.OriginEqual(m, s) : false));
        }
        public static IEnumerable<Renderer> GetCtxRenderers(this ILNUContext ctx, IEnumerable<Renderer> sourceRenderers)
        {
            return ctx.EnumerateRenderer().Where(r => sourceRenderers.Any(s => ctx.OriginEqual(r, s)));
        }
        public static IEnumerable<Renderer> GetCtxRenderers(this ILNUContext ctx, HashSet<Material> ctxMaterials)
        {
            return ctx.EnumerateRenderer().Where(r => r.sharedMaterials.Any(ctxMaterials.Contains));
        }
        public static bool IslilToon(this ILNUContext ctx, Material material)
        {
            var shader = ctx.LookAtGet(material, m => m != null ? m.shader : null);
            return lilToon.lilMaterialUtils.CheckShaderIslilToon(shader);
        }
    }
}
