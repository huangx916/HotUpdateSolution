Shader "Hidden/Unlit/Transparent Colored Cutout ZWriteOff 1"
{
	Properties
	{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
		_AlphaTex ("Alpha (R)", 2D) = "white" {}
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	}

	SubShader
	{
		LOD 200

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"DisableBatching" = "True"
		}
		
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Offset -1, -1
			Fog { Mode Off }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			half4 _MainTex_ST;
			sampler2D _AlphaTex;
			fixed _Cutoff;
			float4 _ClipRange0 = float4(0.0, 0.0, 1.0, 1.0);
			float2 _ClipArgs0 = float2(1000.0, 1000.0);
			float4 _ClipConer = float4(0.0, 0.0, 1000.0, 0.0);

			struct appdata_t
			{
				float4 vertex : POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float2 worldPos : TEXCOORD1;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f o;

			v2f vert (appdata_t v)
			{
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.worldPos = v.vertex.xy * _ClipRange0.zw + _ClipRange0.xy;
				return o;
			}

			half4 frag (v2f IN) : SV_Target
			{
				// Sample the texture
				half4 col = tex2D(_MainTex, IN.texcoord) * IN.color;
				col.a *= tex2D(_AlphaTex, IN.texcoord).r;
				
				if (col.a > 0)
				{
					if (_ClipConer.x > 0 && _ClipConer.y > 0)
					{
						// Softness factor
						float2 factor = max(float2(0, 0), abs(IN.worldPos) + _ClipConer.xy - float2(1.0, 1.0)) / _ClipConer.xy;
						float dis = factor.x * factor.x + factor.y * factor.y;
						if (_ClipConer.w > 0)
						{
							float center = 0.5 + _ClipConer.w * 0.5;
							dis = abs((dis - center) / (1 - center));
						}
						col.a *= clamp((1 - dis) * _ClipConer.z, 0.0, 1.0);
					}
					else
					{
						// Softness factor
						float2 factor = (float2(1.0, 1.0) - abs(IN.worldPos)) * _ClipArgs0;
						col.a *= clamp(min(factor.x, factor.y), 0.0, 1.0);
					}
				}
				
				clip(col.a - _Cutoff);
				return col;
			}
			ENDCG
		}
	}
	
	SubShader
	{
		LOD 100

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"DisableBatching" = "True"
		}
		
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMaterial AmbientAndDiffuse
			
			SetTexture [_MainTex]
			{
				Combine Texture * Primary
			}
		}
	}
}
