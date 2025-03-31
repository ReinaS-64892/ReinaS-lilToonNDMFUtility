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
    internal static class lilToonNormalizeShaderManager
    {
        public static Lazy<Shader> lilToonBaker { get; set; } = new(() => lilToon.lilShaderManager.ltsbaker);
        public static Lazy<Shader> FloatNormalizer  { get; set; } = new(() => Shader.Find("Hidden/LNU/FloatNormalizer"));
        public static Lazy<Shader> ColorNormalizer  { get; set; } = new(() => Shader.Find("Hidden/LNU/ColorNormalizer"));
        public static Lazy<Shader> ColorNormalizerWithDefaultBlack  { get; set; } = new(() => Shader.Find("Hidden/LNU/ColorNormalizerWithDefaultBlack"));
        public static Lazy<Shader> AlphaMaskNormalizer  { get; set; } = new(() => Shader.Find("Hidden/LNU/AlphaMaskNormalizer"));
    }
}
