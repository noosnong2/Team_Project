Shader "KDH/SmoothLambert"
{
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _BumpMap("NormalMap", 2D) = "white" {}
    }
        SubShader
    {
    Tags { "RenderType" = "Opaque" }
    LOD 200

    CGPROGRAM
    #pragma surface surf Test

    sampler2D _MainTex;
    sampler2D _BumpMap;

    struct Input
    {
        float2 uv_MainTex;
        float2 uv_BumpMap;
    };

    void surf(Input IN, inout SurfaceOutput o)
    {
        o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));

        fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
        o.Albedo = c.rgb;

        o.Alpha = c.a;

    }

    float4 LightingTest(SurfaceOutput s,
                         float3 lightDir,
                         float3 viewDir,
                         float atten) {

        float ndotl = saturate(dot(s.Normal, lightDir)) * 0.5 + 0.5;

        float4 final;
        final.rgb = ndotl * s.Albedo * _LightColor0.rgb *atten;
        final.a = s.Alpha;
        return final;
    }

    ENDCG
    }
        FallBack "Diffuse"
}
