using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using nadena.dev.ndmf.preview;
using UnityEngine;

namespace lilToonNDMFUtility
{
    internal static class LNUTempRt
    {
        // TODO : RenderTexture pooling

        public static RenderTexture Get(int width, int height, RenderTextureFormat fmt = RenderTextureFormat.ARGB32)
        {
            return new(width, height, 0, fmt);
        }
        public static void Rel(RenderTexture rt)
        {
            UnityEngine.Object.DestroyImmediate(rt);
        }
    }
    internal static class LNUTempMat
    {
        // TODO : Material pooling

        public static Material Get(Shader shader)
        {
            return new Material(shader);
        }
        public static void Rel(Material mat)
        {
            UnityEngine.Object.DestroyImmediate(mat);
        }
    }
}
