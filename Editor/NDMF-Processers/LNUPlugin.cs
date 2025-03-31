using nadena.dev.ndmf;

[assembly: ExportsPlugin(typeof(lilToonNDMFUtility.LNUPlugin))]

namespace lilToonNDMFUtility
{
    internal class LNUPlugin : Plugin<LNUPlugin>
    {
        protected override void Configure()
        {
            InPhase(BuildPhase.Transforming)
                .Run(lilToonMaterialPropertyUnificatorProcessor.Instance)
                .PreviewingWith(new lilToonMaterialPropertyUnificatorFilter())

                .Then
                .Run(lilToonMaterialNormalizerProcessor.Instance)
                .PreviewingWith(new lilToonMaterialNormalizerFilter())
            ;

            InPhase(BuildPhase.Optimizing)
                .BeforePlugin("com.anatawa12.avatar-optimizer")
                .Run(lilToonMaterialOptimizerProcessor.Instance)

                .Then
                .AfterPlugin("net.rs64.tex-trans-tool")
                .Run("TextureCompression", ctx =>
                {
                    GetCtx(ctx).Compression();
                })

                .Then
                .Run("LNU Component Page", ctx =>
                {
                    foreach (var t in ctx.AvatarRootObject.GetComponentsInChildren<LNUAvatarTagComponent>(true))
                    {
                        UnityEngine.Object.DestroyImmediate(t);
                    }
                })
            ;

        }
        internal static LNUBuildContext GetCtx(BuildContext context)
        {
            return context.GetState<LNUBuildContext>(ndmfCtx => new(ndmfCtx));
        }
    }
}
