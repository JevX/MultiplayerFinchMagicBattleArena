﻿Shader "Finch/Super opimized Particles"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _ColorTex("Color Tex", Color) = (1,1,1,1)
        _ColorAdd("Color Add", Color) = (0.2,0.2,0.2,1)
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }

            Pass
            {
                CGPROGRAM
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

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float4 _ColorTex;
                float4 _ColorAdd;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col = tex2D(_MainTex, i.uv);
                    col = col.r;
                    col *= _ColorTex;

                    return _ColorAdd + col;
                }
                    ENDCG
        }
        }
}