Shader "Unlit/Line"
{
	Properties
	{
		_LineLength("Line Length", Float) = 0
		_LineWidth("Line Width", Float) = 0
		_MainTex("Texture", 2D) = "white" {}

		_MainLineColor("Main Line Color", Color) = (1.0, 0.01, 0.0, 1.0)
		_SecondaryLineColor("Secondary Line Color", Color) = (1.0,0.5,0.0,0.0)
		_HighlightColor("Highlight Color", Color) = (1.0, 0.3, 0.0, 0.0)


		_LineIntensity("Line Intensity", Float) = 1
		_HighlightIntensity("Highlight Intensity", Float) = 1

		[IntRange] _StencilRef("Stencil Reference Value", Range(0,255)) = 0
	}
		SubShader
		{
		  Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		 
			/*
			Tags {
			"RenderType" = "Opaque"
			"Queue" = "Geometry-200"
		}*/


			LOD 100

			Cull Off
			ZWrite Off
			// ZWrite On
			// ZTest Less
			// Blend SrcAlpha One
			 Blend One One


			/*
		Stencil{
			Ref 74
			Comp NotEqual //Always
			Pass Replace
		}
		*/
			/*
		Stencil {
			Ref 81
			Comp NotEqual
		}*/


	   Pass
	   {
			/*
				 Stencil {
					Ref 2
					Comp always
					Pass keep
					Fail decrWrap
					ZFail keep
				}

			*/

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
					float2 uv2 : TEXCOORD1;
					float3 center : NORMAL;
					float4 normal : TANGENT;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					//float3 color : TEXCOORD1;
					float  angle : TEXCOORD2;
					float4 vertex : SV_POSITION;


					//float4 scrPos : TEXCOORD3;
				};

				sampler2D _MainTex;

				float _LineLength;
				float _LineWidth;
				float3 _MainLineColor;	// float3(1, 0.01, 0);
				float3 _SecondaryLineColor;	// float3(1, 0.5, 0)
				float3 	_HighlightColor; // float3(1, 0.3, 0)


				float _LineIntensity;
				float _HighlightIntensity;

				/*
				float rand_1_05(in float2 uv)
				{
					float2 noise = (frac(sin(dot(uv, float2(12.9898, 78.233)*2.0)) * 43758.5453));
					return abs(noise.x + noise.y) * 0.5;
				}
				*/

				v2f vert(appdata v)
				{
					v2f o;

					float fade = saturate(v.uv.y / 0.5);
					fade *= 0.15 + 0.85* saturate((_LineLength - v.uv.y) / 0.1);

					fade = pow(fade, 0.5);

					fade = clamp(fade, 0.2, 1);



					float angle = v.uv2.x;
					float kscale = pow(saturate(1 - abs(angle)*0.025), 2);
					float kscale0 = 1;

					kscale = 0.75f + 0.25f * kscale;


					float scale = 1;
					float speed = 2.0;
					scale = (sin(fade * 15 + speed * _Time.x * 30) *  sin(fade * 20 + speed * _Time.x * 4) *0.5 + 0.5)*0.25
						+ 0.75;
					//scale *=fade;


					//if(frac(_Time.x*20)>0.5)kscale0 = 1;


					float LengthFactor = lerp(0.5, 1, saturate(_LineLength / 0.3));
					scale *= LengthFactor;


					float fadeCorner = saturate(v.uv.y / 0.035);
					fadeCorner *= fadeCorner * fadeCorner;
					//scale *= fadeCorner;


					float3 delta = v.vertex.xyz - v.center.xyz;
					delta *= sign(v.uv2.y - 0.5);//v.uv2.y
					delta = normalize(delta);


					//vertex.xyz -= delta * pow(saturate(1 - fadeCorner),10) * 0.05;


					// Billboarding
					//-----------------
					float3  lForward = normalize(_WorldSpaceCameraPos - v.center);
					float3 normal = v.normal.xyz;
					float3  di = normalize(cross(normal, lForward));

					float3 viewDir = UNITY_MATRIX_IT_MV[2].xyz;
					float NdotV = abs(dot(lForward, viewDir));

					NdotV = saturate(1 - saturate(1 - NdotV) * 20);


					float scaleDown = 0.5;
					/*if (frac(_Time.x * 10) > 0.5)
					{
						scaleDown = 1;
					}
					*/


					//o.color = pow(NdotV ,2.2);

					float3 billboard = v.center - (v.uv.x - 0.5) * di*_LineWidth*scaleDown;
					//v.vertex.xyz = lerp(billboard, v.vertex.xyz, NdotV);

					//if (frac(_Time.x * 20) > 0.5)
					v.vertex.xyz = billboard;
					//-----------------

					v.vertex.xyz -= delta * pow(saturate(1 - fadeCorner), 10) * _LineWidth*0.25;


					float4 vertex = lerp(float4(v.center, 1), v.vertex, scale);

					o.angle = kscale0;

					o.vertex = UnityObjectToClipPos(vertex);
					o.uv = v.uv;
					//o.scrPos = ComputeScreenPos(o.vertex);
					return o;
				}
				/*
				const float3 LUMA_COEFFS = float3(0.2126, 0.7152, 0.0722);


				const float HCV_EPSILON = 1e-10;
				const float HSL_EPSILON = 1e-10;

				float get_luminance(float3 rgb) {
					return dot(LUMA_COEFFS, rgb);
				}

				// Converts from pure Hue to linear RGB
				float3 hue_to_rgb(float hue)
				{
					float R = abs(hue * 6 - 3) - 1;
					float G = 2 - abs(hue * 6 - 2);
					float B = 2 - abs(hue * 6 - 4);
					return saturate(float3(R, G, B));
				}

				// Converts from HSL to linear RGB
				float3 hsl_to_rgb(float3 hsl)
				{
					float3 rgb = hue_to_rgb(hsl.x);
					float C = (1 - abs(2 * hsl.z - 1)) * hsl.y;
					return (rgb - 0.5) * C + hsl.z;
				}

				// Converts a value from linear RGB to HCV (Hue, Chroma, Value)
				float3 rgb_to_hcv(float3 rgb)
				{
					// Based on work by Sam Hocevar and Emil Persson
					float4 P = (rgb.g < rgb.b) ? float4(rgb.bg, -1.0, 2.0 / 3.0) : float4(rgb.gb, 0.0, -1.0 / 3.0);
					float4 Q = (rgb.r < P.x) ? float4(P.xyw, rgb.r) : float4(rgb.r, P.yzx);
					float C = Q.x - min(Q.w, Q.y);
					float H = abs((Q.w - Q.y) / (6 * C + HCV_EPSILON) + Q.z);
					return float3(H, C, Q.x);
				}

				// Converts from linear rgb to HSL
				float3 rgb_to_hsl(float3 rgb)
				{
					float3 HCV = rgb_to_hcv(rgb);
					float L = HCV.z - HCV.y * 0.5;
					float S = HCV.y / (1 - abs(L * 2 - 1) + HSL_EPSILON);
					return float3(HCV.x, S, L);
				}


				float3 Uncharted2ToneMapping(float3 color)
				{
					float gamma = 2.2;

					float A = 0.15;
					float B = 0.50;
					float C = 0.10;
					float D = 0.20;
					float E = 0.02;
					float F = 0.30;
					float W = 11.2;
					float exposure = 2.;
					color *= exposure;
					color = ((color * (A * color + C * B) + D * E) / (color * (A * color + B) + D * F)) - E / F;
					float white = ((W * (A * W + C * B) + D * E) / (W * (A * W + B) + D * F)) - E / F;
					color /= white;
					color = pow(color, 1.0 / gamma);
					return color;
				}
						*/

			
				#define BlendOverlayf(base, blend)     (base < 0.5 ? (2.0 * base * blend) : (1.0 - 2.0 * (1.0 - base) * (1.0 - blend)))
			
				/*
				float find_closest(int x, int y, float c0)
				{
					float Scale = 1.0;

					int dither[8][8] = {
					{ 0, 32, 8, 40, 2, 34, 10, 42}, // 8x8 Bayer ordered dithering //
					{48, 16, 56, 24, 50, 18, 58, 26}, // pattern. Each input pixel //
					{12, 44, 4, 36, 14, 46, 6, 38}, // is scaled to the 0..63 range //
					{60, 28, 52, 20, 62, 30, 54, 22}, // before looking in this table //
					{ 3, 35, 11, 43, 1, 33, 9, 41}, // to determine the action. //
					{51, 19, 59, 27, 49, 17, 57, 25},
					{15, 47, 7, 39, 13, 45, 5, 37},
					{63, 31, 55, 23, 61, 29, 53, 21} };

					float limit = 0.0;
					if (x < 8)
					{
						limit = (dither[x][y] + 1) / 64.0;
					}


					if (c0 < limit)
						return 0.0;
					return 1.0;

				}
				*/
/*
			float4 oldFrag(v2f i)
			{
				//return float4(i.color, 1);

				// sample the texture
				//fixed4 col = tex2D(_MainTex, i.uv);
				// apply fog

				//return float4(i.color, 1);

				float Lk1 = saturate(_LineLength / 0.75);
				float CornerFactor = lerp(4, 1, Lk1);


				float lFadeBrush = lerp(0.1, 0.25, saturate(_LineLength / 0.35));


				float k2 = saturate(_LineLength / 0.5);
				k2 = k2 * k2;

				float lFade = 0.0001 + 0.35 * k2;
				float lFadeBack = 0.000001 + 0.1 * Lk1;
				float fade = saturate(i.uv.y / lFade);


				float invYuv = _LineLength - i.uv.y;

				fade *= 0.35 + 0.65* saturate(invYuv / lFadeBack);


				float fadeCorner = saturate(i.uv.y / 0.04*CornerFactor);
				fadeCorner *= saturate((_LineLength - i.uv.y) / 0.04*CornerFactor);
				fadeCorner *= fadeCorner * fadeCorner;


				float center01 = 1 - abs(i.uv.x - 0.5) * 2;
				float alpha = 0;


				k2 = sin(i.uv.y * 40)*0.5 + 0.5;
				k2 = 1.0 - k2 * k2 * 0.5;

				float fade2 = saturate(i.uv.y / lFadeBrush);
				float mask = tex2D(_MainTex, i.uv.yx * float2(1, 2.0) + float2(0.25 + _Time.x*0.35, 0)).b;
				mask = pow(mask,2.2);
				float maskO = mask;
				mask = saturate(mask*1.5);
				mask = 1 - mask * mask;



				float brush = 1 - fade2 * k2;
				brush = BlendOverlayf(brush, mask);


				float slice = 0.7;
				float bodyK = saturate((center01 - slice) / slice) * fade;
				bodyK *= bodyK * 4;

				float slice2 = 0.75 - (1 - fade)* 0.3 * fade;
				float bodyK2 = saturate((center01 - slice2) / slice2) *(0.75 + fade * 0.25);
				bodyK2 *= bodyK2 * 3 * (1 - brush * 0.75);


				float slice3 = 0.75 - (1 - fade) * 0.75;
				float bodyK3 = saturate((center01 - slice3) / slice3);
				bodyK3 *= bodyK3 * 10 * (1 - brush);



				bodyK = saturate(bodyK);
				bodyK2 = saturate(bodyK2);
				bodyK3 = saturate(bodyK3);

				float3 body = 0;


				body = _MainLineColor;
				body = lerp(body, float3(1, 1, 0), bodyK2 * 0.5);

				float3 colorArrow = lerp(1, _SecondaryLineColor, sqrt(saturate(1 - fade2 * 1.5)));
				body = lerp(body, colorArrow, bodyK3);



				body *= bodyK * fade + bodyK2 * 4;


				float k = sin(_Time.x * 200 + i.uv.y * 20 - sin(i.uv.y * 0.01) * 10000)*0.5 + 0.5;
				k = 0.75 + k * 0.1;


				float highlight = pow(center01, 3.2) * k;
				highlight = max(highlight + brush * bodyK * 0.5, 0);


				body *= _LineIntensity;

				body += i.angle * highlight * _HighlightColor*(0.05 + saturate(fade - 0.25) *1.3) * _HighlightIntensity;



				//if (frac(_Time.x * 10) > 0.5) {
				float kL = pow(saturate(_LineLength * 20), 1.5);
				float fadeCorner2 = saturate(i.uv.y / 0.005);
				fadeCorner = lerp(fadeCorner2, fadeCorner, kL);
				//}
				//body = float3(frac(i.uv.y*10+0.1+_Time.x*10), 0, 0);

				//return float4(lerp(float3(1,0,0), 1, kL), 1);



				body *= fadeCorner;

				//
				//float2 wcoord = (i.scrPos.xy / i.scrPos.w);

				//float3 lumcoeff = float3(0.299, 0.587, 0.114);
				//float luminance = dot(body.rgb, lumcoeff);

				//float2 xy = wcoord * _ScreenParams.xy*float2(1.0, 1);
				//int x = int(xy.x) % 8;
				//int y = int(xy.y) % 8;

				//float r = find_closest(x, y, saturate(luminance*1.5));



				//float r = rand_1_05(i.uv);

				//if (r < 0.5)
				//	discard;
				

				return float4(body*fadeCorner, 1);

			}*/

			float4 frag(v2f i) : SV_Target
			{
				float3 C = 0.5;
				float A = 1;


				//float Lk1 = saturate(_LineLength / 0.75);
				float k2 = saturate(_LineLength / 0.5);
				k2 = k2 * k2;
				float lFade = 0.0001 + 0.35 * k2;
				//float lFadeBack = 0.000001 + 0.1 * Lk1;
				float fade = saturate(i.uv.y / lFade);
				//float invYuv = _LineLength - i.uv.y;
				//fade *= 0.35 + 0.65* saturate(invYuv / lFadeBack);


				float2 pan = float2(0.25 + _Time.x*0.35, 0);
				float4 mask = tex2D(_MainTex, i.uv.yx + pan);



				float lFadeBrush = lerp(0.1, 0.25, saturate(_LineLength / 0.35));
				float fadeWhiteBlade = saturate(i.uv.y / lFadeBrush);

				fade = BlendOverlayf(fade, mask.b);


				fadeWhiteBlade *= mask.a;

				C = lerp(_MainLineColor, _SecondaryLineColor, mask.r);
				C = lerp(C, 1, fadeWhiteBlade);
				C *= mask.r*_LineIntensity;


				C += _HighlightIntensity * _HighlightColor *lerp(mask.r, mask.g, saturate(fade));
				C *= fade;

				/*
				body *= bodyK * fade + bodyK2 * 4;


				float k = sin(_Time.x * 200 + i.uv.y * 20 - sin(i.uv.y * 0.01) * 10000)*0.5 + 0.5;
				k = 0.75 + k * 0.1;


				float highlight = pow(center01, 3.2) * k;
				highlight = max(highlight + brush * bodyK * 0.5, 0);


				body *= _LineIntensity;

				body += i.angle * highlight * _HighlightColor*(0.05 + saturate(fade - 0.25) *1.3) * _HighlightIntensity;
				*/


				/*
				if (frac(_Time.x * 10) > 0.5) 
				{
					return oldFrag(i);
				}
				*/


				return float4(C*A, 1);
			}

			ENDCG
		}
	}
}
