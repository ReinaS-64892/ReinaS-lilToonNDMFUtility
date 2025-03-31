Shader "Hidden/LNU/AlphaMaskNormalizer"
{
    Properties
    {
        // this code from TexTransTool and lilToon
        //https://github.com/ReinaS-64892/TexTransTool/blob/50c97a92347b813c981c904b3c79a23f0a203be3/Runtime/TextureAtlas/AtlasShaderSupport/liltoon/lilToonAtlasBaker.shader
        //https://github.com/lilxyzw/lilToon/blob/d9e1e06d2bc25961f8849ba2dd926ffebcef6bf7/Assets/lilToon/CustomShaderResources/Properties/Default.lilblock

        //----------------------------------------------------------------------------------------------------------------------
        // Alpha Mask
        _AlphaMaskMode              ("sAlphaMaskModes", Int) = 0

        _AlphaMask                  ("AlphaMask", 2D) = "white" {}
        _AlphaMaskScale             ("Scale", Float) = 1
        _AlphaMaskValue             ("Offset", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            uint _AlphaMaskMode;
            sampler2D _AlphaMask;
            float _AlphaMaskScale;
            float _AlphaMaskValue;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                if (_AlphaMaskMode != 0){
                    return saturate(tex2D(_AlphaMask ,i.uv) * _AlphaMaskScale + _AlphaMaskValue);
                }
                else{
                    return float4(1,1,1,1);
                }
            }
            ENDHLSL
        }
    }
}
