Shader "Unlit/Unlit Fresnel"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}

		_MainColor("Main Color", Color) = (1,1,1,1)
		_FresnelColor("Fresnel Color", Color) = (1,1,1,1)
		_FresnelPower("Fresnel Power", Float) = 2
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
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float fresnel : FRESNEL;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			float4 _MainColor;
			float4 _FresnelColor;
			float _FresnelPower;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);

				float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
				float fresnelAmount = 1 - saturate(dot(v.normal, viewDir));
				fresnelAmount = pow(fresnelAmount, _FresnelPower);
				o.fresnel = fresnelAmount;

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				//fixed4 col = tex2D(_MainTex, i.uv);

				fixed4 col = _MainColor;

				float3 outRGB = lerp(col.rgb, _FresnelColor.rgb, _FresnelColor.a * i.fresnel);
				float4 outRGBA = float4(outRGB, col.a);

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, outRGBA);
				return outRGBA;
			}
			ENDCG
		}
	}
}
