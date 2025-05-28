using nadena.dev.ndmf;

[assembly: ExportsPlugin(typeof(lilToonNDMFUtility.LNUPlugin))]

namespace lilToonNDMFUtility
{
    internal class LNUPlugin : Plugin<LNUPlugin>
    {
        public override string QualifiedName => "net.rs64.reina-s-liltoon-ndmf-utility";
        public override string DisplayName => "ReinaS' lilToon NDMF Utility";
        protected override void Configure()
        {
            InPhase(BuildPhase.Transforming)
                .AfterPlugin("net.rs64.tex-trans-tool")
                .Run(lilToonMaterialPropertyUnificatorProcessor.Instance)
                .PreviewingWith(new lilToonMaterialPropertyUnificatorFilter())

                .Then
                .Run(lilToonMaterialNormalizerProcessor.Instance)
                .PreviewingWith(new lilToonMaterialNormalizerFilter())
            ;

            InPhase(BuildPhase.Optimizing)
                .BeforePlugin("com.anatawa12.avatar-optimizer")
                .AfterPlugin("net.rs64.tex-trans-tool")// lilToon の Optimizing は AtlasTexture よりも後に行うべきである。
                .Run(lilToonMaterialOptimizerProcessor.Instance).Then

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
