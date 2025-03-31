Shader "Hidden/LNU/FloatNormalizer"
{
    Properties
    {
        _Use                  ("Use", Int) = 0

        _MaskTex         ("MaskTex", 2D) = "white" {}
        _Value             ("Value", Range(0, 1)) = 1
        _MaxValue    ("MaxValue", float) = 1
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

            uint _Use;
            sampler2D _MaskTex;
            float _Value;
            float _MaxValue;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                if (_Use){
                    return tex2D(_MaskTex,i.uv) * (_Value / _MaxValue);
                }
                else{
                    return float4(0,0,0,0);
                }
            }
            ENDHLSL
        }
    }
}
