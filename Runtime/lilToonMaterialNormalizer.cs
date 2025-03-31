using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace lilToonNDMFUtility
{
    [AddComponentMenu(LNUConsts.ADD_MENU_BASE + nameof(lilToonMaterialNormalizer))]
    public sealed class lilToonMaterialNormalizer : LNUAvatarTagComponent, ITargetMaterialRange
    {
        public MaterialRange NormalizeRange;
        public NormalizeTargetDetectionMode NormalizeTargetDetectionMode;
        // public List<NormalizeOption> NormalizeTargetManualSelections = new();
        // public bool NormalizeUseProperty = true;

        public MaterialRange TargetMaterialRange => NormalizeRange;
    }
    public enum NormalizeTargetDetectionMode
    {
        TextureContains,
        All,

        // セーブデータをうまく扱えなかったので断念
        // Manual = 0,
    }
    // [Serializable]
    // public class NormalizeOption
    // {
    //     public NormalizeTarget Target;
    //     public bool EnableNormalize;
    // }

    public enum NormalizeTarget
    {
        MainTexture = 0,
        // MainColorAdjustMask = 1,

        MainTexture2nd = 2,
        // BlendMask は焼けそうな項目がない
        // MainTexture2ndBlendMask = 3,
        // 動的な変化が前提なこれを焼こうとする事自体が誤っている
        // MainTexture2ndDissolveMask  = 4,

        MainTexture3rd = 5,
        // MainTexture3rdBlendMask = 6,
        // MainTexture3rdDissolveMask = 7,

        AlphaMask = 8,

        // NormalMap の Scale を焼く方法がわからんから断念
        // NormalMap = 9,
        // NormalMap2nd = 10,
        // NormalMap2ndScaleMask = 11,

        // TangentMap は対象がない
        // AnisotropyTangentMap = 12,
        // Scale 系履き方がわからぬ
        // AnisotropyScaleMask = 13,

        BacklightColorTexture = 14,

        ShadowStrengthMask = 15,

        // なんだろう ... これ？ 若干わかる気もするけどわからん。
        // ShadowBorderMask = 16,
        // ShadowBlurMask = 17,

        // これ演算式を見てもうまくベイクする方法がわかんなかったから断念
        // ShadowColorTexture = 18,
        // Shadow2ndColorTexture = 19,
        // Shadow3rdColorTexture = 20,

        RimShadeMask = 21,
        SmoothnessTexture = 22,
        MetallicGlossMap = 23,
        ReflectionColorTexture = 24,

        MatCapBlendMask = 25,
        // MatCapBumpMap = 26,

        MatCap2ndBlendMask = 27,
        // MatCap2ndBumpMap = 28,

        RimColorTexture = 29,

        GlitterColorTexture = 30,

        EmissionMap = 31,
        EmissionBlendMask = 32,

        Emission2ndMap = 33,
        Emission2ndBlendMask = 34,

        // そもそもこれはなんですか？
        // ParallaxMap = 35,

        OutlineTexture = 36,
        OutlineWidthMask = 37,
        // VectorTex も Scale の焼き方がわからない
        // OutlineVectorTexture = 38,
    }
    public static class NormalizeTargetUtil
    {
        static NormalizeTarget[] s_allValue;
        public static IEnumerable<NormalizeTarget> AllTarget()
        {
            return s_allValue ??= Enum.GetValues(typeof(NormalizeTarget)).Cast<NormalizeTarget>().ToArray();
        }

        public static string GetPropertyName(this NormalizeTarget normalizeTarget)
        {
            switch (normalizeTarget)
            {
                default:
                case NormalizeTarget.MainTexture: return "_MainTex";
                // case NormalizeTarget.MainColorAdjustMask: return "_MainColorAdjustMask";

                case NormalizeTarget.MainTexture2nd: return "_Main2ndTex";
                // case NormalizeTarget.MainTexture2ndBlendMask: return "_Main2ndBlendMask";
                // case NormalizeTarget.MainTexture2ndDissolveMask: return "_Main2ndDissolveMask";

                case NormalizeTarget.MainTexture3rd: return "_Main3rdTex";
                // case NormalizeTarget.MainTexture3rdBlendMask: return "_Main3rdBlendMask";
                // case NormalizeTarget.MainTexture3rdDissolveMask: return "_Main3rdDissolveMask";

                case NormalizeTarget.AlphaMask: return "_AlphaMask";

                // case NormalizeTarget.NormalMap: return "_BumpMap";
                // case NormalizeTarget.NormalMap2nd: return "_Bump2ndMap";
                // case NormalizeTarget.NormalMap2ndScaleMask: return "_Bump2ndScaleMask";

                // case NormalizeTarget.AnisotropyTangentMap: return "_AnisotropyTangentMap";
                // case NormalizeTarget.AnisotropyScaleMask: return "_AnisotropyScaleMask";

                case NormalizeTarget.BacklightColorTexture: return "_BacklightColorTex";

                case NormalizeTarget.ShadowStrengthMask: return "_ShadowStrengthMask";

                // case NormalizeTarget.ShadowBorderMask: return "_ShadowBorderMask";
                // case NormalizeTarget.ShadowBlurMask: return "_ShadowBlurMask";

                // case NormalizeTarget.ShadowColorTexture: return "_ShadowColorTex";
                // case NormalizeTarget.Shadow2ndColorTexture: return "_Shadow2ndColorTex";
                // case NormalizeTarget.Shadow3rdColorTexture: return "_Shadow3rdColorTex";

                case NormalizeTarget.RimShadeMask: return "_RimShadeMask";
                case NormalizeTarget.SmoothnessTexture: return "_SmoothnessTex";
                case NormalizeTarget.MetallicGlossMap: return "_MetallicGlossMap";
                case NormalizeTarget.ReflectionColorTexture: return "_ReflectionColorTex";

                case NormalizeTarget.MatCapBlendMask: return "_MatCapBlendMask";
                // case NormalizeTarget.MatCapBumpMap: return "_MatCapBumpMap";

                case NormalizeTarget.MatCap2ndBlendMask: return "_MatCap2ndBlendMask";
                // case NormalizeTarget.MatCap2ndBumpMap: return "_MatCap2ndBumpMap";

                case NormalizeTarget.RimColorTexture: return "_RimColorTex";

                case NormalizeTarget.GlitterColorTexture: return "_GlitterColorTex";

                case NormalizeTarget.EmissionMap: return "_EmissionMap";
                case NormalizeTarget.EmissionBlendMask: return "_EmissionBlendMask";

                case NormalizeTarget.Emission2ndMap: return "_Emission2ndMap";
                case NormalizeTarget.Emission2ndBlendMask: return "_Emission2ndBlendMask";

                // case NormalizeTarget.ParallaxMap: return "_ParallaxMap";

                case NormalizeTarget.OutlineTexture: return "_OutlineTex";
                case NormalizeTarget.OutlineWidthMask: return "_OutlineWidthMask";
                    // case NormalizeTarget.OutlineVectorTexture: return "_OutlineVectorTex";

            }
        }
    }
}
