Shader "CustomShader/Stylus" 
{
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_EmissiveTex("Texture", 2D) = "white" {}
		_RoughnessTex("Texture", 2D) = "white" {}
		_BumpMap("Bumpmap", 2D) = "bump" {}
		_RimColor("Rim Color", Color) = (0.26,0.19,0.16,0.0)
		_RimPower("Rim Power", Range(0.5,8.0)) = 3.0


		_StylusActive("StylusActive", Range(0.0,1.0)) = 0.0
		_EmissionHue("Emission Hue", Float) = 0.0
		
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		CGPROGRAM
			
			#pragma surface surf Standard
			
			struct Input {
				float2 uv_MainTex;
				float2 uv_BumpMap;
				float3 viewDir;
			};
			
			sampler2D _MainTex;
			sampler2D _BumpMap;
			sampler2D _EmissiveTex;
			sampler2D _RoughnessTex;
			float4 _RimColor;
			float _RimPower;
			
			float _StylusActive;
			float _EmissionHue;

			/*
			struct SurfaceOutputStandard
			{
				fixed3 Albedo;      // base (diffuse or specular) color
				fixed3 Normal;      // tangent space normal, if written
				half3 Emission;
				half Metallic;      // 0=non-metal, 1=metal
				half Smoothness;    // 0=rough, 1=smooth
				half Occlusion;     // occlusion (default 1)
				fixed Alpha;        // alpha for transparencies
			};*/


			// Converts the rgb value to hsv, where H's range is -1 to 5
			float3 rgb_to_hsv(float3 RGB)
			{
				float r = RGB.x;
				float g = RGB.y;
				float b = RGB.z;

				float minChannel = min(r, min(g, b));
				float maxChannel = max(r, max(g, b));

				float h = 0;
				float s = 0;
				float v = maxChannel;

				float delta = maxChannel - minChannel;

				if (delta != 0)
				{
					s = delta / v;

					if (r == v) h = (g - b) / delta;
					else if (g == v) h = 2 + (b - r) / delta;
					else if (b == v) h = 4 + (r - g) / delta;
				}

				return float3(h, s, v);
			}

			float3 hsv_to_rgb(float3 HSV)
			{
				float3 RGB = HSV.z;

				float h = HSV.x;
				float s = HSV.y;
				float v = HSV.z;

				float i = floor(h);
				float f = h - i;

				float p = (1.0 - s);
				float q = (1.0 - s * f);
				float t = (1.0 - s * (1 - f));

				if (i == 0) { RGB = float3(1, t, p); }
				else if (i == 1) { RGB = float3(q, 1, p); }
				else if (i == 2) { RGB = float3(p, 1, t); }
				else if (i == 3) { RGB = float3(p, q, 1); }
				else if (i == 4) { RGB = float3(t, p, 1); }
				else /* i == -1 */ { RGB = float3(1, p, q); }

				RGB *= v;

				return RGB;
			}

			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				float4 LUM = tex2D(_EmissiveTex, IN.uv_MainTex);
				float4 R = tex2D(_RoughnessTex, IN.uv_MainTex);

				o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
				o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
				half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));


				float PowerScaling = (sin(_Time.y * 25) + sin(_Time.y * 80)*0.5);
				PowerScaling = 1 + PowerScaling * 0.1;


				o.Emission = _RimColor.rgb * pow(rim, _RimPower*PowerScaling)*_StylusActive;

				o.Smoothness = R.a * 2;


				float3 hsv = rgb_to_hsv(LUM.xyz);
				hsv.x += _EmissionHue;
				// Put the hue back to the -1 to 5 range
				//if (hsv.x > 5) { hsv.x -= 6.0; }
				LUM.rgb = hsv_to_rgb(hsv);


				o.Emission += LUM.rgb*(1 + 0.5*_StylusActive);

			}
		ENDCG
	}
}