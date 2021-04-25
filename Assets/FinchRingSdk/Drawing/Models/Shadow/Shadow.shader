Shader "CustomShader/Shadow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

	_UseColor("UseColor", Float) = 0
		_Color("Color", Color) = (0,0,0,1)

    }
    SubShader
    {
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }

		// inside Pass
		ZWrite Off
Blend DstColor Zero // Multiplicative

        LOD 100

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

			float3 _Color;
			float _UseColor;
            sampler2D _MainTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
				float a = tex2D(_MainTex, i.uv).r;
				
				float3 C = lerp(1, _UseColor<0.5?0:_Color, a);

				return float4(C, 1);
            }
            ENDCG
        }
    }
}
