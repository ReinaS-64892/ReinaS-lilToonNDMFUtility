using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using nadena.dev.ndmf;
using nadena.dev.ndmf.preview;
using UnityEngine;
using UnityEngine.Rendering;

namespace lilToonNDMFUtility
{
    internal sealed class lilToonMaterialNormalizerProcessor : Pass<lilToonMaterialNormalizerProcessor>
    {

        protected override void Execute(BuildContext context)
        {
            var ctx = LNUPlugin.GetCtx(context);
            var normalizers = context.AvatarRootObject.GetComponentsInChildren<lilToonMaterialNormalizer>(true).Where(b => b.enabled);
            if (normalizers.Any() is false) { return; }

            foreach (var normalizer in normalizers)
            {
                DoNormalize(ctx, normalizer);
            }
            ctx.ReadBack();
        }

        internal static void DoNormalize(ILNUContext ctx, lilToonMaterialNormalizer normalizer)
        {
            var targetMaterials = ctx.GetCtxMaterials(normalizer).Where(ctx.IslilToon).ToArray();
            if (targetMaterials.Any() is false) { return; }

            ctx.LookAt(normalizer);

            var mutTargetMaterials = targetMaterials.Select(m => { ctx.GetMutableMaterial(ref m); return m; }).ToArray();


            var normalizingOption = GetNormalizingOptions(normalizer, mutTargetMaterials);
            using var loadCtx = new LoadOriginCtx(ctx);

            if (normalizingOption[NormalizeTarget.MainTexture].EnableNormalize) lilToonNormalizerImpl.DoNormalizeMainTexture(ctx, loadCtx, mutTargetMaterials);
            if (normalizingOption[NormalizeTarget.MainTexture2nd].EnableNormalize) lilToonNormalizerImpl.DoNormalizeMainTexture2nd(ctx, loadCtx, mutTargetMaterials);
            if (normalizingOption[NormalizeTarget.MainTexture3rd].EnableNormalize) lilToonNormalizerImpl.DoNormalizeMainTexture3rd(ctx, loadCtx, mutTargetMaterials);
            if (normalizingOption[NormalizeTarget.AlphaMask].EnableNormalize) lilToonNormalizerImpl.DoNormalizeAlphaMask(ctx, loadCtx, mutTargetMaterials);
            if (normalizingOption[NormalizeTarget.BacklightColorTexture].EnableNormalize) lilToonNormalizerImpl.DoNormalizeBacklightColorTexture(ctx, loadCtx, mutTargetMaterials);
            if (normalizingOption[NormalizeTarget.ShadowStrengthMask].EnableNormalize) lilToonNormalizerImpl.DoNormalizeShadowStrengthMask(ctx, loadCtx, mutTargetMaterials, targetMaterials);
            // if (normalizingOption[NormalizeTarget.ShadowColorTexture].EnableNormalize) lilToonNormalizerImpl.DoNormalizeShadowColorTexture(ctx, loadCtx, mutTargetMaterials);
            // if (normalizingOption[NormalizeTarget.Shadow2ndColorTexture].EnableNormalize) lilToonNormalizerImpl.DoNormalizeShadow2ndColorTexture(ctx, loadCtx, mutTargetMaterials);
            // if (normalizingOption[NormalizeTarget.Shadow3rdColorTexture].EnableNormalize) lilToonNormalizerImpl.DoNormalizeShadow3rdColorTexture(ctx, loadCtx, mutTargetMaterials);
            if (normalizingOption[NormalizeTarget.RimShadeMask].EnableNormalize) lilToonNormalizerImpl.DoNormalizeRimShadeMask(ctx, loadCtx, mutTargetMaterials);
            if (normalizingOption[NormalizeTarget.SmoothnessTexture].EnableNormalize) lilToonNormalizerImpl.DoNormalizeSmoothnessTexture(ctx, loadCtx, mutTargetMaterials, targetMaterials);
            if (normalizingOption[NormalizeTarget.MetallicGlossMap].EnableNormalize) lilToonNormalizerImpl.DoNormalizeMetallicGlossMap(ctx, loadCtx, mutTargetMaterials, targetMaterials);
            if (normalizingOption[NormalizeTarget.ReflectionColorTexture].EnableNormalize) lilToonNormalizerImpl.DoNormalizeReflectionColorTexture(ctx, loadCtx, mutTargetMaterials, targetMaterials);
            if (normalizingOption[NormalizeTarget.MatCapBlendMask].EnableNormalize) lilToonNormalizerImpl.DoNormalizeMatCapBlendMask(ctx, loadCtx, mutTargetMaterials);
            if (normalizingOption[NormalizeTarget.MatCap2ndBlendMask].EnableNormalize) lilToonNormalizerImpl.DoNormalizeMatCap2ndBlendMask(ctx, loadCtx, mutTargetMaterials);
            if (normalizingOption[NormalizeTarget.RimColorTexture].EnableNormalize) lilToonNormalizerImpl.DoNormalizeRimColorTexture(ctx, loadCtx, mutTargetMaterials);
            if (normalizingOption[NormalizeTarget.GlitterColorTexture].EnableNormalize) lilToonNormalizerImpl.DoNormalizeGlitterColorTexture(ctx, loadCtx, mutTargetMaterials);
            if (normalizingOption[NormalizeTarget.EmissionMap].EnableNormalize) lilToonNormalizerImpl.DoNormalizeEmissionMap(ctx, loadCtx, mutTargetMaterials, targetMaterials);
            if (normalizingOption[NormalizeTarget.EmissionBlendMask].EnableNormalize) lilToonNormalizerImpl.DoNormalizeEmissionBlendMask(ctx, loadCtx, mutTargetMaterials, targetMaterials);
            if (normalizingOption[NormalizeTarget.Emission2ndMap].EnableNormalize) lilToonNormalizerImpl.DoNormalizeEmission2ndMap(ctx, loadCtx, mutTargetMaterials, targetMaterials);
            if (normalizingOption[NormalizeTarget.Emission2ndBlendMask].EnableNormalize) lilToonNormalizerImpl.DoNormalizeEmission2ndBlendMask(ctx, loadCtx, mutTargetMaterials, targetMaterials);
            if (normalizingOption[NormalizeTarget.OutlineTexture].EnableNormalize) lilToonNormalizerImpl.DoNormalizeOutlineTexture(ctx, loadCtx, mutTargetMaterials);
            if (normalizingOption[NormalizeTarget.OutlineWidthMask].EnableNormalize) lilToonNormalizerImpl.DoNormalizeOutlineWidthMask(ctx, loadCtx, mutTargetMaterials);
        }

        private static Dictionary<NormalizeTarget, NormalizingOption> GetNormalizingOptions(lilToonMaterialNormalizer normalizer, Material[] mutlilToonMaterials)
        {
            //TODO : LookAt
            var optionDict = new Dictionary<NormalizeTarget, NormalizingOption>();


            switch (normalizer.NormalizeTargetDetectionMode)
            {
                default:
                // case NormalizeTargetDetectionMode.Manual:
                //     {
                //         var selections = new Dictionary<NormalizeTarget, NormalizeOption>();
                //         foreach (var opt in normalizer.NormalizeTargetManualSelections.Reverse<NormalizeOption>())
                //         {
                //             selections[opt.Target] = opt;
                //         }
                //         foreach (var target in NormalizeTargetUtil.AllTarget())
                //         {
                //             if (selections.TryGetValue(target, out var opt) is false)
                //             {
                //                 optionDict[target] = new() { EnableNormalize = false, };
                //                 continue;
                //             }

                //             optionDict[target] = new() { EnableNormalize = opt.EnableNormalize, };
                //         }
                //         break;
                //     }
                case NormalizeTargetDetectionMode.TextureContains:
                    {
                        foreach (var target in NormalizeTargetUtil.AllTarget())
                            optionDict[target] = new() { EnableNormalize = ContainsTexture(mutlilToonMaterials, target.GetPropertyName()), };
                        break;
                    }
                case NormalizeTargetDetectionMode.All:
                    {
                        foreach (var target in NormalizeTargetUtil.AllTarget())
                            optionDict[target] = new() { EnableNormalize = true, };
                        break;
                    }
            }
            return optionDict;
        }
        internal static bool ContainsTexture(Material[] materials, string propertyName)
        {
            foreach (var mat in materials)
            {
                if (mat.HasTexture(propertyName) is false) { continue; }
                var tex = mat.GetTexture(propertyName);
                if (tex == null) { continue; }

                return true;
            }
            return false;
        }

        class NormalizingOption
        {
            public bool EnableNormalize;
        }

    }
    internal sealed class lilToonMaterialNormalizerFilter : LNUPreviewFilter<lilToonMaterialNormalizer>
    {
        public override Task<IRenderFilterNode> Instantiate(RenderGroup group, IEnumerable<(Renderer, Renderer)> proxyPairs, ComputeContext context)
        {
            var ctx = new LNUPreviewContext(context, proxyPairs.Select(i => i.Item2).ToArray());
            var data = group.GetData<FilterPassingData>();
            foreach (var unificator in data.targetComponents.OrderBy(c => data.componentExecutingOrder[c]))
            {
                lilToonMaterialNormalizerProcessor.DoNormalize(ctx, unificator);
            }
            return Task.FromResult<IRenderFilterNode>(new PreviewContextRecallerNode(ctx));
        }
    }

    internal class LoadOriginCtx : IDisposable
    {
        private ILNUContext ctx;
        Dictionary<Texture, RenderTexture> _originCash = new();

        public LoadOriginCtx(ILNUContext ctx)
        {
            this.ctx = ctx;
        }

        public RenderTexture GetOriginTexture(Material mat, string propName)
        {
            if (mat.HasTexture(propName) is false) { return null; }
            var tex = mat.GetTexture(propName);
            if (tex == null) { return null; }

            if (_originCash.TryGetValue(tex, out var rt) is false)
            {
                rt = LNUTempRt.Get(tex.width, tex.height);
                if (tex is Texture2D texture2D) ctx.LoadTexture2D(rt, texture2D);
                else Graphics.Blit(tex, rt);
                _originCash[tex] = rt;
            }
            return rt;
        }

        public void Dispose() { foreach (var rt in _originCash.Values) { LNUTempRt.Rel(rt); } }
    }

    public static class lilToonNormalizerImpl
    {
        internal static void DoNormalizeMainTexture(ILNUContext ctx, LoadOriginCtx loadTexCtx, Material[] mutlilToonMaterials)
        {
            var colorDefaultValue = new LNUMaterialProperty()
            {
                PropertyName = lilPropC.mainColor,
                PropertyType = ShaderPropertyType.Texture,
                ColorValue = Color.white,
            };
            var hsvgAdjustDefaultValue = new LNUMaterialProperty()
            {
                PropertyName = lilPropC.mainTexHSVG,
                PropertyType = ShaderPropertyType.Vector,
                ColorValue = new Vector4(0f, 1f, 1f, 1f),
            };
            var gradationMapDefaultValue = new LNUMaterialProperty()
            {
                PropertyName = lilPropC.mainGradationStrength,
                PropertyType = ShaderPropertyType.Range,
                FloatValue = 0f,
            };

            var baker = LNUTempMat.Get(lilToonNormalizeShaderManager.lilToonBaker.Value);

            foreach (var mat in mutlilToonMaterials)
            {
                if (mat.GetTexture(lilPropC.mainTex) != null
                    && colorDefaultValue.EqualFromMaterial(mat, false)
                    && hsvgAdjustDefaultValue.EqualFromMaterial(mat, false)
                    && gradationMapDefaultValue.EqualFromMaterial(mat, false))
                    continue;

                var bakedRt = GenerateBakedTarget(mat, lilPropC.mainTex);

                var mainTex = loadTexCtx.GetOriginTexture(mat, lilPropC.mainTex);
                baker.SetTexture(lilPropC.mainTex, mainTex);
                baker.SetColor(lilPropC.mainColor, mat.GetColor(lilPropC.mainColor));
                baker.SetVector(lilPropC.mainTexHSVG, mat.GetVector(lilPropC.mainTexHSVG));
                baker.SetFloat(lilPropC.mainGradationStrength, mat.GetFloat(lilPropC.mainGradationStrength));
                baker.SetTexture(lilPropC.mainGradationTex, loadTexCtx.GetOriginTexture(mat, lilPropC.mainGradationTex));
                baker.SetTexture(lilPropC.mainColorAdjustMask, loadTexCtx.GetOriginTexture(mat, lilPropC.mainColorAdjustMask));

                Graphics.Blit(mainTex, bakedRt, baker);

                ctx.RegisterReadBack2Replace(mat.GetTexture(lilPropC.mainTex), bakedRt);
                mat.SetTexture(lilPropC.mainTex, bakedRt);

                mat.SetColor(lilPropC.mainColor, Color.white);
                mat.SetVector(lilPropC.mainTexHSVG, new Vector4(0f, 1f, 1f, 1f));
                mat.SetFloat(lilPropC.mainGradationStrength, 0f);
                mat.SetTexture(lilPropC.mainGradationTex, null);
                mat.SetTexture(lilPropC.mainColorAdjustMask, null);
            }
        }
        private static void ColorNormalize(ILNUContext ctx, LoadOriginCtx loadTexCtx, Material[] mutlilToonMaterials,
            string use,
            string tex,
            string col,
            Material[] originlilToonMaterials = null,
            bool defaultTexBlack = false
        )
        {
            var useDefault = new LNUMaterialProperty()
            {
                PropertyName = use,
                PropertyType = ShaderPropertyType.Int,
                IntValue = 1,
            };
            var colDefault = new LNUMaterialProperty()
            {
                PropertyName = col,
                PropertyType = ShaderPropertyType.Color,
                ColorValue = Color.white,
            };

            var baker = new Material(defaultTexBlack ? lilToonNormalizeShaderManager.ColorNormalizerWithDefaultBlack.Value : lilToonNormalizeShaderManager.ColorNormalizer.Value);


            for (var i = 0; mutlilToonMaterials.Length > i; i += 1)
            {
                var refMat = originlilToonMaterials?[i] ?? mutlilToonMaterials[i];
                var mat = mutlilToonMaterials[i];
                if (mat.GetTexture(tex) != null
                    && useDefault.EqualFromMaterial(mat, false)
                    && colDefault.EqualFromMaterial(mat, false)) continue;

                var bakedRt = GenerateBakedTarget(mat, tex);

                baker.SetInt(lilPropC.CN_Use, refMat.GetInt(use));
                baker.SetTexture(lilPropC.CN_Tex, loadTexCtx.GetOriginTexture(refMat, tex));
                baker.SetColor(lilPropC.CN_Col, refMat.GetColor(col));

                Graphics.Blit(null, bakedRt, baker);

                ctx.RegisterReadBack2Replace(mat.GetTexture(tex), bakedRt);
                mat.SetTexture(tex, bakedRt);

                mat.SetInt(use, 1);
                mat.SetColor(col, Color.white);
            }
        }
        private static void FloatNormalize(ILNUContext ctx, LoadOriginCtx loadTexCtx, Material[] mutlilToonMaterials,
            string use,
            string tex,
            string val,
            Material[] originlilToonMaterials = null
        )
        {
            var useDefault = new LNUMaterialProperty()
            {
                PropertyName = use,
                PropertyType = ShaderPropertyType.Int,
                IntValue = 1,
            };
            var floatDefault = new LNUMaterialProperty()
            {
                PropertyName = val,
                PropertyType = ShaderPropertyType.Float,
                FloatValue = 1f,
            };

            var baker = new Material(lilToonNormalizeShaderManager.FloatNormalizer.Value);

            for (var i = 0; mutlilToonMaterials.Length > i; i += 1)
            {
                var refMat = originlilToonMaterials?[i] ?? mutlilToonMaterials[i];
                var mat = mutlilToonMaterials[i];
                if (mat.GetTexture(tex) != null
                    && useDefault.EqualFromMaterial(mat, false)
                    && floatDefault.EqualFromMaterial(mat, false)) continue;

                var bakedRt = GenerateBakedTarget(mat, tex);

                baker.SetInt(lilPropC.FN_Use, refMat.GetInt(use));
                baker.SetTexture(lilPropC.FN_Tex, loadTexCtx.GetOriginTexture(refMat, tex));
                baker.SetFloat(lilPropC.FN_Val, refMat.GetFloat(val));
                baker.SetFloat(lilPropC.FN_MVal, 1f);

                Graphics.Blit(null, bakedRt, baker);

                ctx.RegisterReadBack2Replace(mat.GetTexture(tex), bakedRt);
                mat.SetTexture(tex, bakedRt);

                mat.SetInt(use, 1);
                mat.SetFloat(val, 1f);
            }
        }
        internal static void DoNormalizeMainTexture2nd(ILNUContext ctx, LoadOriginCtx loadTexCtx, Material[] mutlilToonMaterials)
        {
            ColorNormalize(ctx, loadTexCtx, mutlilToonMaterials,
                lilPropC.useMain2ndTex,
                lilPropC.main2ndTex,
                lilPropC.mainColor2nd
            );
        }


        internal static void DoNormalizeMainTexture3rd(ILNUContext ctx, LoadOriginCtx loadTexCtx, Material[] mutlilToonMaterials)
        {
            ColorNormalize(ctx, loadTexCtx, mutlilToonMaterials,
                lilPropC.useMain3rdTex,
                lilPropC.main3rdTex,
                lilPropC.mainColor3rd
            );
        }

        internal static void DoNormalizeAlphaMask(ILNUContext ctx, LoadOriginCtx loadTexCtx, Material[] mutlilToonMaterials)
        {
            var alphaMode = new LNUMaterialProperty()
            {
                PropertyName = lilPropC.alphaMaskMode,
                PropertyType = ShaderPropertyType.Int,
                IntValue = 0,
            };
            var alphaMaskScaleDefault = new LNUMaterialProperty()
            {
                PropertyName = lilPropC.alphaMaskScale,
                PropertyType = ShaderPropertyType.Color,
                FloatValue = 1f,
            };
            var alphaMaskValueDefault = new LNUMaterialProperty()
            {
                PropertyName = lilPropC.alphaMaskValue,
                PropertyType = ShaderPropertyType.Color,
                FloatValue = 0f,
            };

            var baker = new Material(lilToonNormalizeShaderManager.AlphaMaskNormalizer.Value);

            foreach (var mat in mutlilToonMaterials)
            {
                if (mat.GetTexture(lilPropC.alphaMask) != null
                    && alphaMode.EqualFromMaterial(mat, false) is false
                    && alphaMaskScaleDefault.EqualFromMaterial(mat, false)
                    && alphaMaskValueDefault.EqualFromMaterial(mat, false)) continue;

                var bakedRt = GenerateBakedTarget(mat, lilPropC.alphaMask);

                baker.SetTexture(lilPropC.alphaMask, loadTexCtx.GetOriginTexture(mat, lilPropC.alphaMask));
                baker.SetInt(lilPropC.alphaMaskMode, mat.GetInt(lilPropC.alphaMaskMode));
                baker.SetFloat(lilPropC.alphaMaskScale, mat.GetFloat(lilPropC.alphaMaskScale));
                baker.SetFloat(lilPropC.alphaMaskValue, mat.GetFloat(lilPropC.alphaMaskValue));

                Graphics.Blit(null, bakedRt, baker);


                ctx.RegisterReadBack2Replace(mat.GetTexture(lilPropC.alphaMask), bakedRt);
                mat.SetTexture(lilPropC.alphaMask, bakedRt);

                if (mat.GetInt(lilPropC.alphaMaskMode) == 0) { mat.SetInt(lilPropC.alphaMaskMode, 2); }
                mat.SetFloat(lilPropC.alphaMaskScale, 1f);
                mat.SetFloat(lilPropC.alphaMaskValue, 0f);

            }
        }

        internal static void DoNormalizeBacklightColorTexture(ILNUContext ctx, LoadOriginCtx loadTexCtx, Material[] mutlilToonMaterials)
        {
            ColorNormalize(
                ctx, loadTexCtx, mutlilToonMaterials,
                lilPropC.useBacklight,
                lilPropC.backlightColorTex,
                lilPropC.backlightColor
            );
        }

        internal static void DoNormalizeShadowStrengthMask(ILNUContext ctx, LoadOriginCtx loadTexCtx, Material[] mutlilToonMaterials, Material[] originMaterials)
        {
            FloatNormalize(
                ctx, loadTexCtx, mutlilToonMaterials,
                lilPropC.useShadow,
                lilPropC.shadowStrengthMask,
                lilPropC.shadowStrength,
                originMaterials
            );
        }

        // internal static void DoNormalizeShadowColorTexture(ILNUContext ctx, LoadOriginCtx loadTexCtx, Material[] mutlilToonMaterials,Material[] originMaterials)
        // {
        //     ColorNormalize(
        //         ctx, loadTexCtx, mutlilToonMaterials,
        //         lilPropC.useShadow,
        //         lilPropC.shadowColorTex,
        //         lilPropC.shadowColor,
        //         originMaterials,
        //         true
        //     );
        // }
        // internal static void DoNormalizeShadow2ndColorTexture(ILNUContext ctx, LoadOriginCtx loadTexCtx, Material[] mutlilToonMaterials,Material[] originMaterials)
        // {
        //     ColorNormalize(
        //         ctx, loadTexCtx, mutlilToonMaterials,
        //         lilPropC.useShadow,
        //         lilPropC.shadow2ndColorTex,
        //         lilPropC.shadow2ndColor,
        //         originMaterials,
        //         true
        //     );
        // }
        // internal static void DoNormalizeShadow3rdColorTexture(ILNUContext ctx, LoadOriginCtx loadTexCtx, Material[] mutlilToonMaterials,Material[] originMaterials)
        // {
        //     ColorNormalize(
        //         ctx, loadTexCtx, mutlilToonMaterials,
        //         lilPropC.useShadow,
        //         lilPropC.shadow3rdColorTex,
        //         lilPropC.shadow3rdColor,
        //         originMaterials,
        //         true
        //     );
        // }

        internal static void DoNormalizeRimShadeMask(ILNUContext ctx, LoadOriginCtx loadTexCtx, Material[] mutlilToonMaterials)
        {
            ColorNormalize(
                ctx, loadTexCtx, mutlilToonMaterials,
                lilPropC.useRimShade,
                lilPropC.rimShadeMask,
                lilPropC.rimShadeColor
            );
        }
        internal static void DoNormalizeSmoothnessTexture(ILNUContext ctx, LoadOriginCtx loadTexCtx, Material[] mutlilToonMaterials, Material[] originMaterials)
        {
            FloatNormalize(
                ctx, loadTexCtx, mutlilToonMaterials,
                lilPropC.useReflection,
                lilPropC.smoothnessTex,
                lilPropC.smoothness,
                originMaterials
            );
        }
        internal static void DoNormalizeMetallicGlossMap(ILNUContext ctx, LoadOriginCtx loadTexCtx, Material[] mutlilToonMaterials, Material[] originMaterials)
        {
            FloatNormalize(
                ctx, loadTexCtx, mutlilToonMaterials,
                lilPropC.useReflection,
                lilPropC.metallicGlossMap,
                lilPropC.metallic,
                originMaterials
            );
        }
        internal static void DoNormalizeReflectionColorTexture(ILNUContext ctx, LoadOriginCtx loadTexCtx, Material[] mutlilToonMaterials, Material[] originMaterials)
        {
            ColorNormalize(
                ctx, loadTexCtx, mutlilToonMaterials,
                lilPropC.useReflection,
                lilPropC.reflectionColorTex,
                lilPropC.reflectionColor,
                originMaterials
            );
        }

        internal static void DoNormalizeMatCapBlendMask(ILNUContext ctx, LoadOriginCtx loadTexCtx, Material[] mutlilToonMaterials)
        {
            FloatNormalize(
                ctx, loadTexCtx, mutlilToonMaterials,
                lilPropC.useMatCap,
                lilPropC.matcapBlendMask,
                lilPropC.matcapBlend
            );
        }

        internal static void DoNormalizeMatCap2ndBlendMask(ILNUContext ctx, LoadOriginCtx loadTexCtx, Material[] mutlilToonMaterials)
        {
            FloatNormalize(
                ctx, loadTexCtx, mutlilToonMaterials,
                lilPropC.useMatCap2nd,
                lilPropC.matcap2ndBlendMask,
                lilPropC.matcap2ndBlend
            );
        }

        internal static void DoNormalizeRimColorTexture(ILNUContext ctx, LoadOriginCtx loadTexCtx, Material[] mutlilToonMaterials)
        {
            ColorNormalize(
                ctx, loadTexCtx, mutlilToonMaterials,
                lilPropC.useRim,
                lilPropC.rimColorTex,
                lilPropC.rimColor
            );
        }

        internal static void DoNormalizeGlitterColorTexture(ILNUContext ctx, LoadOriginCtx loadTexCtx, Material[] mutlilToonMaterials)
        {
            ColorNormalize(
                ctx, loadTexCtx, mutlilToonMaterials,
                lilPropC.useGlitter,
                lilPropC.glitterColorTex,
                lilPropC.glitterColor
            );
        }

        internal static void DoNormalizeEmissionMap(ILNUContext ctx, LoadOriginCtx loadTexCtx, Material[] mutlilToonMaterials, Material[] originMaterials)
        {
            ColorNormalize(
                ctx, loadTexCtx, mutlilToonMaterials,
                lilPropC.useEmission,
                lilPropC.emissionMap,
                lilPropC.emissionColor,
                originMaterials
            );
        }
        internal static void DoNormalizeEmissionBlendMask(ILNUContext ctx, LoadOriginCtx loadTexCtx, Material[] mutlilToonMaterials, Material[] originMaterials)
        {
            FloatNormalize(
                ctx, loadTexCtx, mutlilToonMaterials,
                lilPropC.useEmission,
                lilPropC.emissionBlendMask,
                lilPropC.emissionBlend,
                originMaterials
            );
        }

        internal static void DoNormalizeEmission2ndMap(ILNUContext ctx, LoadOriginCtx loadTexCtx, Material[] mutlilToonMaterials, Material[] originMaterials)
        {
            ColorNormalize(
                ctx, loadTexCtx, mutlilToonMaterials,
                lilPropC.useEmission2nd,
                lilPropC.emission2ndMap,
                lilPropC.emission2ndColor,
                originMaterials
            );
        }
        internal static void DoNormalizeEmission2ndBlendMask(ILNUContext ctx, LoadOriginCtx loadTexCtx, Material[] mutlilToonMaterials, Material[] originMaterials)
        {
            FloatNormalize(
                ctx, loadTexCtx, mutlilToonMaterials,
                lilPropC.useEmission2nd,
                lilPropC.emission2ndBlendMask,
                lilPropC.emission2ndBlend,
                originMaterials
            );
        }


        internal static void DoNormalizeOutlineTexture(ILNUContext ctx, LoadOriginCtx loadTexCtx, Material[] mutlilToonMaterials)
        {
            var colorDefaultValue = new LNUMaterialProperty()
            {
                PropertyName = lilPropC.outlineColor,
                PropertyType = ShaderPropertyType.Texture,
                ColorValue = Color.white,
            };
            var hsvgAdjustDefaultValue = new LNUMaterialProperty()
            {
                PropertyName = lilPropC.outlineTexHSVG,
                PropertyType = ShaderPropertyType.Vector,
                ColorValue = new Vector4(0f, 1f, 1f, 1f),
            };

            var baker = LNUTempMat.Get(lilToonNormalizeShaderManager.lilToonBaker.Value);

            foreach (var mat in mutlilToonMaterials)
            {
                // outline を持ってない場合はどうすればいいのかな？
                if (mat.shader.name.Contains("Outline") is false) { continue; }

                if (mat.GetTexture(lilPropC.mainTex) != null
                    && colorDefaultValue.EqualFromMaterial(mat, false)
                    && hsvgAdjustDefaultValue.EqualFromMaterial(mat, false))
                    continue;

                var bakedRt = GenerateBakedTarget(mat, lilPropC.outlineTex);

                var mainTex = loadTexCtx.GetOriginTexture(mat, lilPropC.outlineTex);
                baker.SetTexture(lilPropC.mainTex, mainTex);
                baker.SetColor(lilPropC.mainColor, mat.GetColor(lilPropC.outlineColor));
                baker.SetVector(lilPropC.mainTexHSVG, mat.GetVector(lilPropC.outlineTexHSVG));

                baker.SetFloat(lilPropC.mainGradationStrength, 0f);
                baker.SetTexture(lilPropC.mainGradationTex, null);
                baker.SetTexture(lilPropC.mainColorAdjustMask, null);

                Graphics.Blit(mainTex, bakedRt, baker);

                ctx.RegisterReadBack2Replace(mat.GetTexture(lilPropC.outlineTex), bakedRt);
                mat.SetTexture(lilPropC.outlineTex, bakedRt);

                mat.SetColor(lilPropC.outlineColor, Color.white);
                mat.SetVector(lilPropC.outlineTexHSVG, new Vector4(0f, 1f, 1f, 1f));
            }
        }
        internal static void DoNormalizeOutlineWidthMask(ILNUContext ctx, LoadOriginCtx loadTexCtx, Material[] mutlilToonMaterials)
        {
            var tex = lilPropC.outlineWidthMask;
            var val = lilPropC.outlineWidth;
            var maxValue = mutlilToonMaterials.Max(m => m.shader.name.Contains("Outline") && m.HasProperty(val) ? m.GetFloat(val) : 0f);
            var floatDefault = new LNUMaterialProperty()
            {
                PropertyName = lilPropC.outlineWidth,
                PropertyType = ShaderPropertyType.Float,
                FloatValue = maxValue,
            };

            var baker = new Material(lilToonNormalizeShaderManager.FloatNormalizer.Value);

            foreach (var mat in mutlilToonMaterials)
            {
                // 上に同じ
                if (mat.shader.name.Contains("Outline") is false) { continue; }
                if (mat.GetTexture(tex) != null && floatDefault.EqualFromMaterial(mat, false)) continue;

                var bakedRt = GenerateBakedTarget(mat, tex);

                baker.SetInt(lilPropC.FN_Use, 1);
                baker.SetTexture(lilPropC.FN_Tex, loadTexCtx.GetOriginTexture(mat, tex));
                baker.SetFloat(lilPropC.FN_Val, mat.GetFloat(val));
                baker.SetFloat(lilPropC.FN_MVal, maxValue);

                Graphics.Blit(null, bakedRt, baker);

                ctx.RegisterReadBack2Replace(mat.GetTexture(tex), bakedRt);
                mat.SetTexture(tex, bakedRt);

                mat.SetFloat(val, maxValue);
            }
        }




        private static RenderTexture GenerateBakedTarget(Material mat, string propertyName)
        {
            var tex = mat.GetTexture(propertyName);
            RenderTexture rt = null;
            if (tex != null)
            {
                rt = LNUTempRt.Get(tex.width, tex.height);
                rt.name = tex.name + "(LNU baked)";
            }
            else
            {
                rt = LNUTempRt.Get(4, 4);
                rt.name = propertyName + "(LNU baked-empty)";
            }
            return rt;
        }

        internal static bool AllEqualProperty(Material[] materials, string propertyName)
        {
            LNUMaterialProperty? firstProp = null;
            foreach (var mat in materials)
            {
                Debug.Assert(LNUMaterialProperty.TryGet(mat, propertyName, out var lnuProp));
                firstProp ??= lnuProp;

                if (firstProp.Value.Equals(lnuProp, false)) continue;
                return false;
            }
            return true;
        }
        internal static bool AllEqualProperty(Material[] materials, LNUMaterialProperty firstProp)
        {
            foreach (var mat in materials)
            {
                Debug.Assert(LNUMaterialProperty.TryGet(mat, firstProp.PropertyName, out var lnuProp));
                if (firstProp.Equals(lnuProp, false)) continue;
                return false;
            }
            return true;
        }
        internal static bool ContainsEqualProperty(Material[] materials, LNUMaterialProperty firstProp)
        {
            foreach (var mat in materials)
            {
                Debug.Assert(LNUMaterialProperty.TryGet(mat, firstProp.PropertyName, out var lnuProp));
                if (firstProp.Equals(lnuProp, false)) return true;
            }
            return false;
        }
        internal static bool ContainsAllTexture(Material[] materials, string propertyName)
        {
            foreach (var mat in materials)
            {
                if (mat.HasTexture(propertyName) is false) { continue; }
                var tex = mat.GetTexture(propertyName);
                if (tex != null) { continue; }
                return false;
            }
            return true;
        }
        internal static class lilPropC
        {
            public const string CN_Use = "_Use";
            public const string CN_Tex = "_ColorTex";
            public const string CN_Col = "_Color";

            public const string FN_Use = "_Use";
            public const string FN_Tex = "_MaskTex";
            public const string FN_Val = "_Value";
            public const string FN_MVal = "_MaxValue";

            public const string mainColor = "_Color";
            public const string mainTex = "_MainTex";
            public const string mainTexHSVG = "_MainTexHSVG";
            public const string mainGradationStrength = "_MainGradationStrength";
            public const string mainGradationTex = "_MainGradationTex";
            public const string mainColorAdjustMask = "_MainColorAdjustMask";


            public const string useMain2ndTex = "_UseMain2ndTex";
            public const string mainColor2nd = "_Color2nd";
            public const string main2ndTex = "_Main2ndTex";

            public const string useMain3rdTex = "_UseMain3rdTex";
            public const string mainColor3rd = "_Color3rd";
            public const string main3rdTex = "_Main3rdTex";


            public const string alphaMaskMode = "_AlphaMaskMode";
            public const string alphaMask = "_AlphaMask";
            public const string alphaMaskScale = "_AlphaMaskScale";
            public const string alphaMaskValue = "_AlphaMaskValue";

            public const string useBacklight = "_UseBacklight";
            public const string backlightColor = "_BacklightColor";
            public const string backlightColorTex = "_BacklightColorTex";

            public const string useShadow = "_UseShadow";
            public const string shadowStrength = "_ShadowStrength";
            public const string shadowStrengthMask = "_ShadowStrengthMask";
            public const string shadowColor = "_ShadowColor";
            public const string shadowColorTex = "_ShadowColorTex";
            public const string shadow2ndColor = "_Shadow2ndColor";
            public const string shadow2ndColorTex = "_Shadow2ndColorTex";
            public const string shadow3rdColor = "_Shadow3rdColor";
            public const string shadow3rdColorTex = "_Shadow3rdColorTex";

            public const string useRimShade = "_UseRimShade";
            public const string rimShadeColor = "_RimShadeColor";
            public const string rimShadeMask = "_RimShadeMask";


            public const string useEmission = "_UseEmission";
            public const string emissionColor = "_EmissionColor";
            public const string emissionMap = "_EmissionMap";

            public const string emissionBlend = "_EmissionBlend";
            public const string emissionBlendMask = "_EmissionBlendMask";

            public const string useEmission2nd = "_UseEmission2nd";
            public const string emission2ndColor = "_Emission2ndColor";
            public const string emission2ndMap = "_Emission2ndMap";

            public const string emission2ndBlend = "_Emission2ndBlend";
            public const string emission2ndBlendMask = "_Emission2ndBlendMask";

            public const string useReflection = "_UseReflection";
            public const string metallic = "_Metallic";
            public const string metallicGlossMap = "_MetallicGlossMap";
            public const string smoothness = "_Smoothness";
            public const string smoothnessTex = "_SmoothnessTex";
            public const string reflectionColor = "_ReflectionColor";
            public const string reflectionColorTex = "_ReflectionColorTex";

            public const string useMatCap = "_UseMatCap";
            public const string matcapBlend = "_MatCapBlend";
            public const string matcapBlendMask = "_MatCapBlendMask";

            public const string useMatCap2nd = "_UseMatCap2nd";
            public const string matcap2ndBlend = "_MatCap2ndBlend";
            public const string matcap2ndBlendMask = "_MatCap2ndBlendMask";

            public const string useRim = "_UseRim";
            public const string rimColor = "_RimColor";
            public const string rimColorTex = "_RimColorTex";

            public const string useGlitter = "_UseGlitter";
            public const string glitterColor = "_GlitterColor";
            public const string glitterColorTex = "_GlitterColorTex";

            public const string outlineColor = "_OutlineColor";
            public const string outlineTex = "_OutlineTex";
            public const string outlineTexHSVG = "_OutlineTexHSVG";

            public const string outlineWidth = "_OutlineWidth";
            public const string outlineWidthMask = "_OutlineWidthMask";
        }
    }
}
