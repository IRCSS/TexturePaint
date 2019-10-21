Shader "Unlit/TexturePaint"
{
	

	SubShader
	{
		// =====================================================================================================================
		// TAGS AND SETUP ------------------------------------------
		Tags { "RenderType"="Opaque" }
		LOD	   100
		ZTest  Off
		ZWrite Off
		Cull   Off

		Pass
		{
			CGPROGRAM
			// =====================================================================================================================
			// DEFINE AND INCLUDE ----------------------------------
			#pragma vertex   vert
			#pragma fragment frag

			
			#include "UnityCG.cginc"

			// =====================================================================================================================
			// DECLERANTIONS ----------------------------------
			struct appdata
			{
				float4 vertex   : POSITION;
				float2 uv	    : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				float3 worldPos : TEXCOORD0;
				float2 uv       : TEXCOORD1;
			};

			float4    _Mouse;
			float4x4  mesh_Object2World;
			sampler2D _MainTex;
			float4	  _BrushColor;
			float	  _BrushOpacity;
			float	  _BrushHardness;
			float	  _BrushSize;

			// =====================================================================================================================
			// VERTEX FRAGMENT ----------------------------------

			v2f vert (appdata v)
			{
				v2f o;

				float2 uvRemapped   = v.uv.xy;
				       uvRemapped.y = 1. - uvRemapped.y;
					   uvRemapped   = uvRemapped *2. - 1.;

					   o.vertex     = float4(uvRemapped.xy, 0., 1.);
				       o.worldPos   = mul(mesh_Object2World, v.vertex);
				       o.uv         = v.uv;

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float4 col  = tex2D(_MainTex, i.uv);
				float  size = _BrushSize;
				float  soft = _BrushHardness;
				float  f	= distance(_Mouse.xyz, i.worldPos);
					   f    = 1.-smoothstep(size*soft, size, f);
				
					   col  = lerp(col, _BrushColor, f * _Mouse.w * _BrushOpacity);
					   col  = saturate(col);

				return col;
			}
			ENDCG
		}

	}
}
