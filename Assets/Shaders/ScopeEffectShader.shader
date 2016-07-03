Shader "Sprites/ScopeEffectShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_SampleDistance ("Sample distance", Float) = 1
		_SampleStrength ("Sample strength", Float) = 2
		_MinCenterDistance ("Min center distance", Float) = .15
		_Color("Tint", Color) = (1,1,1,1)
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.0
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float _SampleDistance;
			float _SampleStrength;
			float _MinCenterDistance;

			static const float center = .5;

			fixed4 _Color;

			fixed3 frag (v2f i) : SV_Target
			{
				float samples[10];
				samples[0] = -0.08;
				samples[1] = -0.05;
				samples[2] = -0.03;
				samples[3] = -0.02;
				samples[4] = -0.01;
				samples[5] = 0.01;
				samples[6] = 0.02;
				samples[7] = 0.03;
				samples[8] = 0.05;
				samples[9] = 0.08;

				float2 dir = center - i.uv;
				float dist = max(0, sqrt(dir.x * dir.x + dir.y * dir.y) - _MinCenterDistance);
				dir = dir / dist;

				float4 color = tex2D(_MainTex, i.uv);
				float4 sum = color;

				for (int n = 0; n < 10; n++)
					sum += tex2D(_MainTex, i.uv + dir * samples[n] * _SampleDistance);

				sum /= 11.0;
				float t = dist * _SampleStrength;
				t = clamp(t, 0, 1.0);

				return lerp(color, sum, t) * _Color;
			}
			ENDCG
		}
	}
}
