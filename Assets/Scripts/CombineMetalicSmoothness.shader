Shader "Unlit/CombineMetalicSmoothness"
{
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
		//----------------------------------------
			CGPROGRAM
			#pragma  vertex vert
			#pragma  fragment frag
			
			#include "UnityCG.cginc"
		//----------------------------------------
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv     : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv     : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;								   // contains glossines, passed on by the Graphic.Blit as input
			sampler2D _Smoothness;

		//----------------------------------------

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv     = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col		  = tex2D(_MainTex,    i.uv);   // Metalic is in red channel should go to red channel
				fixed4 smoothness = tex2D(_Smoothness, i.uv);	// Smoothness is in red channel should go to alpha
					   col.a	  = smoothness.r;
				return col;
			}
			ENDCG
		}
	}
}
