using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using nadena.dev.ndmf;
using nadena.dev.ndmf.preview;
using UnityEngine;

namespace lilToonNDMFUtility
{
    internal sealed class lilToonMaterialOptimizerProcessor : Pass<lilToonMaterialOptimizerProcessor>
    {

        protected override void Execute(BuildContext context)
        {
            var ctx = LNUPlugin.GetCtx(context);
            var optimizers = context.AvatarRootObject.GetComponentsInChildren<lilToonMaterialOptimizer>(true).Where(b => b.enabled);
            if (optimizers.Any() is false) { return; }
            DoOptimization(ctx, optimizers);
        }
        public static void DoOptimization(LNUBuildContext ctx, IEnumerable<lilToonMaterialOptimizer> lilToonMaterialOptimizers)
        {
            var ndmfCtx = ctx.NDMFBuildContext;
            var optimizeUnusedPropertyRemove = true;
#if LNU_AAO_AVATAR_OPTIMIZER
            optimizeUnusedPropertyRemove = false;
#endif

            var targetMaterials = lilToonMaterialOptimizers.SelectMany(o => ctx.GetCtxMaterials(o)).Distinct();
            var targetMutableMaterials = targetMaterials.Select(m => { ctx.GetMutableMaterial(ref m); return m; }).ToArray();

            lilycalProcessor.Optimizer.OptimizeMaterials(ndmfCtx, targetMutableMaterials, optimizeUnusedPropertyRemove);
        }
    }

}
