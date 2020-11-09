Shader "Custom/Lamert_Rimlight"
{
	properties
	{
		_MainTex("Albedo (RGB)", 2D) = "white"{}
		_RimColor("Color", Color) = (1,0,0,1)
		_RimPower("Rim Power", Range(0,10)) = 3
	}

	SubShader
	{
		CGPROGRAM

		#pragma surface surf Lambert noambient

		sampler2D _MainTex;
		sampler2D _BumpMap;

		half3 _RimColor;
		half _RimPower;

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 viewDir;

		};

		void surf(Input IN, inout SurfaceOutput o)
		{
			half4 c = tex2D(_MainTex, IN.uv_MainTex);
			half rim = saturate(dot(o.Normal, IN.viewDir));

			o.Albedo = c.rgb;
			o.Emission = pow(1 - rim, _RimPower) * _RimColor.rgb;
		}

		ENDCG
	}
}