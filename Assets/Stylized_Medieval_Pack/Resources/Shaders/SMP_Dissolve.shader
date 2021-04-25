// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SMP/Dissolve"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.45
		_DissolveTexture("Dissolve Texture", 2D) = "white" {}
		_Tiling("Tiling", Float) = 1
		[HDR]_EmissiveColor("Emissive Color", Color) = (4,0.6901961,0,0)
		_EmissiveSharpness("Emissive Sharpness", Range( 0 , 10)) = 5
		_EmissivePresence("Emissive Presence", Range( 0 , 10)) = 4
		_OpacityControl("Opacity Control", Range( -2 , 0.5)) = -0.06
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
		};

		uniform float4 _EmissiveColor;
		uniform float _EmissivePresence;
		uniform sampler2D _DissolveTexture;
		uniform float _Tiling;
		uniform float _EmissiveSharpness;
		uniform float _OpacityControl;
		uniform float _Cutoff = 0.45;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float VertexColorAlpha72 = i.vertexColor.a;
			float2 temp_cast_0 = (_Tiling).xx;
			float2 uv_TexCoord14 = i.uv_texcoord * temp_cast_0;
			float4 tex2DNode16 = tex2D( _DissolveTexture, uv_TexCoord14 );
			float4 temp_cast_1 = (_EmissiveSharpness).xxxx;
			float4 clampResult67 = clamp( pow( ( VertexColorAlpha72 * _EmissivePresence * tex2DNode16 ) , temp_cast_1 ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			float4 lerpResult60 = lerp( ( _EmissiveColor + i.vertexColor ) , i.vertexColor , clampResult67);
			o.Emission = lerpResult60.rgb;
			o.Alpha = 1;
			clip( saturate( ( ( tex2DNode16 + (_OpacityControl + (VertexColorAlpha72 - 0.0) * (0.6 - _OpacityControl) / (1.0 - 0.0)) ) * VertexColorAlpha72 ) ).r - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17000
-1913;29;1906;1004;1549.943;515.2272;1;True;True
Node;AmplifyShaderEditor.RangedFloatNode;28;-2303.084,527.4596;Float;False;Property;_Tiling;Tiling;2;0;Create;True;0;0;False;0;1;0.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;7;-902.5659,-108.4821;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;72;-674.399,50.15047;Float;False;VertexColorAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;14;-2114.359,508.4454;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;16;-1839.336,479.3594;Float;True;Property;_DissolveTexture;Dissolve Texture;1;0;Create;True;0;0;False;0;b494557f5cfe0fc4b8e81b40f73d8be2;b494557f5cfe0fc4b8e81b40f73d8be2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;73;-1576.408,838.9169;Float;False;72;VertexColorAlpha;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;75;-1330.019,76.20137;Float;False;72;VertexColorAlpha;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;66;-1350.738,205.0379;Float;False;Property;_EmissivePresence;Emissive Presence;5;0;Create;True;0;0;False;0;4;5;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;71;-1589.837,949.2086;Float;False;Property;_OpacityControl;Opacity Control;6;0;Create;True;0;0;False;0;-0.06;0;-2;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;50;-1275.057,852.682;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;0.6;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;-995.9111,181.4992;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;64;-1113.605,361.114;Float;False;Property;_EmissiveSharpness;Emissive Sharpness;4;0;Create;True;0;0;False;0;5;10;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;79;-875.9425,-293.2272;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;40;-941.1727,-493.4655;Float;False;Property;_EmissiveColor;Emissive Color;3;1;[HDR];Create;True;0;0;False;0;4,0.6901961,0,0;10.42028,0.5278178,0.05455652,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;63;-771.1942,187.4729;Float;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;21.62;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;74;-985.5082,840.5177;Float;False;72;VertexColorAlpha;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;49;-1020.98,701.6218;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;78;-578.9425,-385.2272;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;76;-470.9344,-41.40995;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;67;-545.94,188.5816;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;70;-711.256,704.9022;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;60;-291.7391,87.19817;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;69;-510.6391,707.1114;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1.151968,9.215743;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;SMP/Dissolve;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.45;True;True;0;True;TransparentCutout;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;1;False;-1;1;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;72;0;7;4
WireConnection;14;0;28;0
WireConnection;16;1;14;0
WireConnection;50;0;73;0
WireConnection;50;3;71;0
WireConnection;65;0;75;0
WireConnection;65;1;66;0
WireConnection;65;2;16;0
WireConnection;63;0;65;0
WireConnection;63;1;64;0
WireConnection;49;0;16;0
WireConnection;49;1;50;0
WireConnection;78;0;40;0
WireConnection;78;1;79;0
WireConnection;76;0;7;0
WireConnection;67;0;63;0
WireConnection;70;0;49;0
WireConnection;70;1;74;0
WireConnection;60;0;78;0
WireConnection;60;1;76;0
WireConnection;60;2;67;0
WireConnection;69;0;70;0
WireConnection;0;2;60;0
WireConnection;0;10;69;0
ASEEND*/
//CHKSM=DF2DB7A661F15A690F3C4D7AE0766B687872A59B