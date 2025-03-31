using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using nadena.dev.ndmf.preview;
using UnityEngine;

namespace lilToonNDMFUtility
{
    internal static class LNURenderTextureUtil
    {
        public static void LNUColorFill(this RenderTexture tex, Color color)
        {
            using (new RtActive(tex))
                GL.Clear(false, true, color);
        }

    }
    public class RtActive : IDisposable
    {
        private readonly RenderTexture preRt;

        public RtActive(RenderTexture rt)
        {
            preRt = RenderTexture.active;
            RenderTexture.active = rt;
        }
        public void Dispose()
        {
            RenderTexture.active = preRt;
        }
    }

}
