// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/test"
{
    Properties
    {
        g_fAabbMin       ("Aabb Min", Float) = -0.5
        g_fAabbMax       ("Aabb Max", Float) = 0.5
        g_bDebugAABB     ("Debug Aabb", Float) = 1.0
        g_fDissolveAlpha ("Dissolve Alpha", Float) = 1.0
        g_fDissolveRange ("Dissolve Range", Float) = 0.35
        g_vOrigin        ("Origin", Vector) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
		Blend One One // Additive
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
                float alpha : TEXCOORD1;               
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
 
            float g_fAabbMin;
            float g_fAabbMax;
            float g_bDebugAABB;
           
            float g_fDissolveAlpha;
            float g_fDissolveRange;
 
            float3 g_vOrigin;
 
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                float3 worldPos = mul (unity_ObjectToWorld, v.vertex).xyz;
                float delta_y = worldPos.y - g_vOrigin.y;
               
                float h = saturate((delta_y - g_fAabbMin) / (g_fAabbMax - g_fAabbMin)); // linstep (inverse lerp)
                float alpha =  saturate( (g_fDissolveAlpha * 2.0 - h) / g_fDissolveRange);
               
                if(g_bDebugAABB > 0.5)
                {
                    alpha = h;
                }
               
                o.alpha = alpha;
               
               
                return o;
            }
 
            float4 frag (v2f i) : SV_Target
            {
                float3 C = 1;
                float  A = i.alpha;
               
               
                if(g_bDebugAABB > 0.5)
                {
                    C = A;
                    A = 1;
                }
				else{
					C = A;
				}
               
                return float4(C, A);
            }
        ENDCG
        }
    }
}