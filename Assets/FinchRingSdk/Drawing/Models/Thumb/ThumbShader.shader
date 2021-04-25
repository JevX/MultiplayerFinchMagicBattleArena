Shader "CustomShader/Thumb"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}


		_NormalTex("Normal (RGB)", 2D) = "white" {}
		_RoughnessTex("Roughness (RGB)", 2D) = "white" {}

		_EmissiveTex("EmissiveTex (RGB)", 2D) = "white" {}

		_NoiseTex("Noise Tex (RGB)", 2D) = "white" {}



		_Hover("Hover", Range(0,1)) = 1.0
		_Thumb("Thumb", Range(0,1)) = 1.0

		_HoverMax("Hover Max", Float) = 0.75
		_ThumbMax("Thumb Max", Float) = 0.5


        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

		sampler2D _MainTex;

		sampler2D _NormalTex;
		sampler2D _RoughnessTex;

		sampler2D _NoiseTex;
		sampler2D _EmissiveTex;

        struct Input
        {
			float2 uv_MainTex;
			float2 uv_NoiseTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;


		half _Hover;
		half _Thumb;

		half _HoverMax;
		half _ThumbMax;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            
			float noise = tex2D(_NoiseTex, IN.uv_NoiseTex + _Time.x).r;
			noise = lerp(0.25,3, noise);


			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			fixed4 R = tex2D(_RoughnessTex, IN.uv_MainTex);


			fixed4 E = tex2D(_EmissiveTex, IN.uv_MainTex);

			E = E * (_Hover*_HoverMax*noise + _Thumb * _ThumbMax);


            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness*R.a;
            o.Alpha = 1;

			o.Normal = UnpackNormal(tex2D(_NormalTex, IN.uv_MainTex));

			o.Emission = E.rgb;

        }
        ENDCG
    }
    FallBack "Diffuse"
}
