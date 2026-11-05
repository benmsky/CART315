// Night sky filled with fog, with the moon glowing softly through it.
Shader "CART315/MoonFog"
{
    Properties
    {
        _MoonDir ("Moon Direction", Vector) = (-0.243, 0.342, -0.908, 0)
    }

    SubShader
    {
        Tags { "Queue" = "Background" "RenderType" = "Background" "PreviewType" = "Skybox" }
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float3 _MoonDir;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 dir : TEXCOORD0;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.dir = v.vertex.xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 dir = normalize(i.dir);
                float angle = acos(clamp(dot(dir, normalize(_MoonDir)), -1.0, 1.0));

                // a bright core and a wide haze, the way light spreads out through fog
                float glow = 0.60 * exp(-angle / 0.13) + 0.32 * exp(-angle / 0.50);
                float moon = smoothstep(0.055, 0.025, angle) * 0.55;
                float3 light = float3(0.30, 0.50, 0.95) * glow + float3(0.85, 0.92, 1.0) * moon;

                // fades out at the horizon so the sky meets the foggy ground cleanly
                float3 col = unity_FogColor.rgb + light * smoothstep(0.0, 0.2, dir.y);

                // a tiny bit of noise stops dark gradients from showing bands
                float grain = frac(sin(dot(i.pos.xy, float2(12.9898, 78.233))) * 43758.5453) - 0.5;
                return fixed4(col + grain / 255.0, 1.0);
            }
            ENDCG
        }
    }
    Fallback Off
}
