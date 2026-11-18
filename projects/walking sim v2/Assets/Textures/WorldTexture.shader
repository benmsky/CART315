// Tiles a texture by real-world size, so it never stretches on scaled cubes and cylinders.
// Each side of an object gets the texture projected onto it from the matching direction.
Shader "CART315/WorldTexture"
{
    Properties
    {
        _Color ("Tint", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
        _BumpMap ("Bump Texture", 2D) = "bump" {}
        _Scale ("Metres Per Tile", Float) = 2
        _BumpStrength ("Bump Strength", Float) = 1
        _Glossiness ("Shine", Range(0, 1)) = 0.1
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.5
        #include "UnityStandardUtils.cginc"

        sampler2D _MainTex, _BumpMap;
        fixed4 _Color;
        float _Scale, _BumpStrength, _Glossiness;

        struct Input
        {
            float3 worldPos;
            float3 worldNormal;
            INTERNAL_DATA
        };

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            o.Normal = float3(0, 0, 1);
            float3 n = WorldNormalVector(IN, float3(0, 0, 1));

            // how much each of the three projections counts on this part of the surface
            float3 blend = pow(abs(n), 6.0);
            blend /= dot(blend, 1.0);

            float3 p = IN.worldPos / _Scale;
            float2 uvX = p.zy, uvY = p.xz, uvZ = p.xy;

            fixed3 albedo = tex2D(_MainTex, uvX).rgb * blend.x
                          + tex2D(_MainTex, uvY).rgb * blend.y
                          + tex2D(_MainTex, uvZ).rgb * blend.z;

            float3 bumpX = UnpackScaleNormal(tex2D(_BumpMap, uvX), _BumpStrength);
            float3 bumpY = UnpackScaleNormal(tex2D(_BumpMap, uvY), _BumpStrength);
            float3 bumpZ = UnpackScaleNormal(tex2D(_BumpMap, uvZ), _BumpStrength);

            // bends the surface direction by the bump on each projection
            bumpX = float3(bumpX.xy + n.zy, abs(bumpX.z) * n.x);
            bumpY = float3(bumpY.xy + n.xz, abs(bumpY.z) * n.y);
            bumpZ = float3(bumpZ.xy + n.xy, abs(bumpZ.z) * n.z);
            float3 worldBump = normalize(bumpX.zyx * blend.x + bumpY.xzy * blend.y + bumpZ.xyz * blend.z);

            // Unity's lighting wants the direction relative to the surface
            float3x3 toTangent = float3x3(
                WorldNormalVector(IN, float3(1, 0, 0)),
                WorldNormalVector(IN, float3(0, 1, 0)),
                WorldNormalVector(IN, float3(0, 0, 1)));

            o.Albedo = albedo * _Color.rgb;
            o.Normal = normalize(mul(toTangent, worldBump));
            o.Smoothness = _Glossiness;
            o.Metallic = 0;
        }
        ENDCG
    }
    Fallback "Diffuse"
}
