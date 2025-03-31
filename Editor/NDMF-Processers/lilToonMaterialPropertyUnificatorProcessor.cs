using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using nadena.dev.ndmf;
using nadena.dev.ndmf.preview;
using UnityEngine;

namespace lilToonNDMFUtility
{
    internal sealed class lilToonMaterialPropertyUnificatorProcessor : Pass<lilToonMaterialPropertyUnificatorProcessor>
    {
        protected override void Execute(BuildContext context)
        {
            var ctx = LNUPlugin.GetCtx(context);
            var unificatorArray = context.AvatarRootObject.GetComponentsInChildren<lilToonMaterialPropertyUnificator>(true).Where(b => b.enabled);
            if (unificatorArray.Any() is false) { return; }
            foreach (var u in unificatorArray) DoUnification(ctx, u);
            ctx.ReadBack();
        }


        public static void DoUnification(ILNUContext ctx, lilToonMaterialPropertyUnificator lilToonMaterialPropertyUnificator)
        {
            ctx.LookAt(lilToonMaterialPropertyUnificator);

            var referenceMaterial = lilToonMaterialPropertyUnificator.UnificationReferenceMaterial;
            ctx.LookAt(referenceMaterial);

            //TODO : error report
            if (referenceMaterial == null) { return; }
            if (referenceMaterial.shader == null) { return; }

            var referenceMaterialShader = referenceMaterial.shader;

            var targetMaterials = ctx.GetCtxMaterials(lilToonMaterialPropertyUnificator);
            var targetMutableMaterials = targetMaterials.Select(m => { ctx.GetMutableMaterial(ref m); return m; }).ToArray();

            foreach (var propName in lilToonMaterialPropertyUnificator.UnificationTargetProperties.Distinct())
            {
                if (LNUMaterialProperty.TryGet(referenceMaterial, propName, out var matProp))
                    foreach (var m in targetMutableMaterials)
                        matProp.TrySet(m);
            }
        }
    }
    internal sealed class lilToonMaterialPropertyUnificatorFilter : LNUPreviewFilter<lilToonMaterialPropertyUnificator>
    {
        public override Task<IRenderFilterNode> Instantiate(RenderGroup group, IEnumerable<(Renderer, Renderer)> proxyPairs, ComputeContext context)
        {
            var ctx = new LNUPreviewContext(context, proxyPairs.Select(i => i.Item2).ToArray());
            var data = group.GetData<FilterPassingData>();
            foreach (var unificator in data.targetComponents.OrderBy(c => data.componentExecutingOrder[c]))
            {
                lilToonMaterialPropertyUnificatorProcessor.DoUnification(ctx, unificator);
            }
            return Task.FromResult<IRenderFilterNode>(new PreviewContextRecallerNode(ctx));
        }
    }
}
