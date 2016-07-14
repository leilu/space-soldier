Shader "Effects/TimeDilationShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_WaveCoreOffset("Core Offset", Float) = .15
		_TimeMultiplier("Time Multiplier", Float) = 1.5
		_DeltaTime("Delta Time", Float) = 0
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

			sampler2D _MainTex;
			float _WaveCoreOffset;
			float _TimeMultiplier;
			float _DeltaTime;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target
			{
				float2 center = float2(.5, .5);
				float d = distance(i.uv, center);
				float t = _DeltaTime * _TimeMultiplier;

				float diff = d - t;
				diff *= .75;
				float m = clamp(1 - abs(diff * 1 / (_WaveCoreOffset * clamp(_DeltaTime / .2, 0, 1))), 0, 1);
				m *= diff;
				
				float2 diffUV = normalize(i.uv - center);

				return tex2D(_MainTex, i.uv + diffUV * m);
			}
			ENDCG
		}
	}
}
