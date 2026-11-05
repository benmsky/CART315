// The forest floor: grass everywhere, with dirt showing wherever the Ground Mask is bright.
Shader "CART315/GroundTexture"
{
    Properties
    {
        _GrassTex ("Grass", 2D) = "white" {}
        _GrassBump ("Grass Bump", 2D) = "bump" {}
        _DirtTex ("Dirt", 2D) = "white" {}
        _DirtBump ("Dirt Bump", 2D) = "bump" {}
        _Mask ("Ground Mask", 2D) = "black" {}
        _GrassScale ("Metres Per Grass Tile", Float) = 3
        _DirtScale ("Metres Per Dirt Tile", Float) = 4
        _MapSize ("Map Size In Metres", Float) = 140
        _GrassBumpStrength ("Grass Bump Strength", Float) = 1
        _DirtBumpStrength ("Dirt Bump Strength", Float) = 0.3
        _GrassTint ("Grass Tint", Color) = (1, 1.3, 0.75, 1)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        CGPROGRAM
        #pragma surface surf Lambert fullforwardshadows
        #pragma target 3.5
        #include "UnityStandardUtils.cginc"

        sampler2D _GrassTex, _GrassBump, _DirtTex, _DirtBump, _Mask;
        float _GrassScale, _DirtScale, _MapSize, _GrassBumpStrength, _DirtBumpStrength;
        float4 _GrassTint;

        struct Input
        {
            float3 worldPos;
            float3 worldNormal;
            INTERNAL_DATA
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            o.Normal = float3(0, 0, 1);
            float2 world = IN.worldPos.xz;

            float2 grassUV = world / _GrassScale, dirtUV = world / _DirtScale;
            float3 grass = tex2D(_GrassTex, grassUV).rgb * _GrassTint.rgb;
            float3 dirt = tex2D(_DirtTex, dirtUV).rgb * 1.8; // a little brighter so the path stands out

            // big, soft light and dark patches hide the texture repeating
            float patches = tex2D(_GrassTex, world / 37.0).g * 3.0 + tex2D(_DirtTex, world / 83.0).r * 2.0;
            grass *= lerp(0.65, 1.2, saturate(patches));

            float mask = tex2D(_Mask, world / _MapSize + 0.5).r;
            float3 bump = lerp(
                UnpackScaleNormal(tex2D(_GrassBump, grassUV), _GrassBumpStrength),
                UnpackScaleNormal(tex2D(_DirtBump, dirtUV), _DirtBumpStrength),
                mask);

            float3 worldBump = normalize(float3(bump.x, bump.z, bump.y));
            float3x3 toTangent = float3x3(
                WorldNormalVector(IN, float3(1, 0, 0)),
                WorldNormalVector(IN, float3(0, 1, 0)),
                WorldNormalVector(IN, float3(0, 0, 1)));

            o.Albedo = lerp(grass, dirt, mask);
            o.Normal = normalize(mul(toTangent, worldBump));
        }
        ENDCG
    }
    Fallback "Diffuse"
}
