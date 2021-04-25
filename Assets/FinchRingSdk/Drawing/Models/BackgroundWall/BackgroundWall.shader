// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "CustomShader/BackgroundWall"
{
    Properties
    {
		_MainTex("Texture", 2D) = "white" {}
		_Noise ("Noise", 2D) = "white" {}

		_TileX("TileX", Float) = 1.0
		_TileY("TileY", Float) = 1.0
		
			_Intensity("Intensity", Float) = 1.0
			_Power("Power", Float) = 1.0



    }
    SubShader
    {
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        LOD 100

		ZWrite Off
		Blend One One
		Cull Off


		Stencil {
			Ref 81
			Comp NotEqual
		}


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
				float3 wpos : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

			sampler2D _Noise;


			float _TileX;
			float _TileY;

			float _Intensity;
			float _Power;

            v2f vert (appdata v)
            {
                v2f o;

				float3 wpos = mul(unity_ObjectToWorld, v.vertex);
				o.wpos = wpos;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = wpos.xy * _TileX+float2(0, _TileY);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
				float fade = (i.wpos.y + 0.5)/0.01;
				fade = saturate(fade);
				fade = fade * fade;

				float3 pos = mul(unity_ObjectToWorld, float4(0, 0, 0, 1));
				float L = length(pos.xy - i.wpos.xy);
				L = 1- pow(saturate(L),0.5);

				fade *= L;

                // sample the texture
				float3 C = tex2D(_MainTex, i.uv).rgb;
				C = pow(C, _Power);
				C *= _Intensity* fade;


				float noiseK = tex2D(_Noise, i.wpos.xy*0.5 + i.wpos.z *100 + float2(0,1)*_Time.x*2.0);

				C *= 0.05+1.0*pow(noiseK,2.2);

                return float4(C, 1);
            }
            ENDCG
        }
    }
}
