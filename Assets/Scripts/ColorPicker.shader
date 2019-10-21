Shader "Unlit/ColorPicker"
{
	SubShader
	{
		// =====================================================================================================================
		// TAGS AND SETUP ------------------------------------------
		Tags { "RenderType"="Opaque" }
		LOD 100


		// =====================================================================================================================
		// _____________________________ PASS ONE _________________________________________	// this pass draws the Saturation Value box
		Pass
		{
		// =====================================================================================================================
		// DEFINE AND INCLUDE ----------------------------------
			CGPROGRAM
			#pragma	 vertex   vert
			#pragma  fragment frag
			
			#include "UnityCG.cginc"

			// =====================================================================================================================
			// DECLERANTIONS ----------------------------------
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

			    float _Hue;

	
			// =====================================================================================================================
			// Helper Function ----------------------------------
			float3 hsv2rgb(in float3 c)
			{
				float3 rgb = clamp(abs(fmod(c.x*6.0 + float3(0.0, 4.0, 2.0), 6.0) - 3.0) - 1.0, 0.0, 1.0);
				return c.z * lerp(float3(1.0, 1., 1.), rgb, c.y);
			}

			// =====================================================================================================================
			// VERTEX FRAGMENT ----------------------------------
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv     = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col     = float4(0.,0.,0.,1.);
			           col.xyz = hsv2rgb(float3(_Hue,i.uv.xy));
				return col;
			}
			ENDCG
		}

		// =====================================================================================================================
		// _____________________________ PASS TWO _________________________________________	// this pass draws the Hue box
		Pass
			{
				// =====================================================================================================================
				// DEFINE AND INCLUDE ----------------------------------
				CGPROGRAM
				#pragma vertex   vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				// =====================================================================================================================
				// DECLERANTIONS ----------------------------------
				struct appdata
			{
				float4 vertex : POSITION;
				float2 uv	  : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv	  : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};


				float _Value;
				float _Saturation;

				// =====================================================================================================================
				// Helper Function ----------------------------------
			float3 hsv2rgb(in float3 c)
			{
				float3 rgb = clamp(abs(fmod(c.x*6.0 + float3(0.0, 4.0, 2.0), 6.0) - 3.0) - 1.0, 0.0, 1.0);
				return c.z * lerp(float3(1.0,1.,1.), rgb, c.y);
			}

			// =====================================================================================================================
			// VERTEX FRAGMENT ----------------------------------
			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv	 = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col	   = float4(0.,0.,0.,1.);
					   col.xyz = hsv2rgb(float3(i.uv.x, _Saturation, 1.0));
				return col;
			}
				ENDCG
			}


	}
}
