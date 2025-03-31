Shader "Hidden/LNU/ColorNormalizerWithDefaultBlack"
{
    Properties
    {
        _Use                  ("Use", Int) = 0

        _ColorTex         ("ColorTex", 2D) = "black" {}
        _Color             ("Color", Color) = (0,0,0,0)
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
            sampler2D _ColorTex;
            float4 _Color;

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
                    return tex2D(_ColorTex,i.uv) * _Color;
                }
                else{
                    return float4(0,0,0,0);
                }
            }
            ENDHLSL
        }
    }
}
