Shader "Unlit/UpperLowerCutoff"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_LowerCutoff("Lower Cutoff", Range(0,1)) = 0
		_UpperCutoff("Upper Cutoff", Range(0,1)) = 0.5
		_Gamma("Gamma", Float) = 2.25
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			half _LowerCutoff;
			half _UpperCutoff;
			half _Gamma;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);

				clip(col.a - 0.5);

				half value = col.r;
				value = pow(value, _Gamma);

				if (_LowerCutoff > _UpperCutoff)
				{
					if (value >= _UpperCutoff && value <= _LowerCutoff) { clip(-1); }
				}
				else
				{
					if (value <= _LowerCutoff || value >= _UpperCutoff) { clip(-1); }
				}

				return (1,1,1,1);
			}
			ENDCG
		}
	}
}
