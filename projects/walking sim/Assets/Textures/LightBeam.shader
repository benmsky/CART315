// A soft cone of headlight shining through the fog.
// Brightest at the lamp, fading along its length, softer at its edges and where it touches things.
Shader "CART315/LightBeam"
{
    Properties
    {
        _Color ("Beam Colour", Color) = (1, 0.9, 0.7, 1)
        _Strength ("Strength", Float) = 0.35
        _EdgeSoftness ("Edge Softness", Float) = 2
        _SoftDistance ("Soft Edge Distance", Float) = 1
    }

    SubShader
    {
        Tags { "Queue" = "Transparent+10" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
        Blend One One
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"

            fixed4 _Color;
            float _Strength, _EdgeSoftness, _SoftDistance;
            UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

            struct v2f
            {
                float4 pos : SV_POSITION;
                float along : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float3 view : TEXCOORD2;
                float4 screen : TEXCOORD3;
                UNITY_FOG_COORDS(4)
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.along = v.texcoord.y;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.view = _WorldSpaceCameraPos - mul(unity_ObjectToWorld, v.vertex).xyz;
                o.screen = ComputeScreenPos(o.pos);
                COMPUTE_EYEDEPTH(o.screen.z);
                UNITY_TRANSFER_FOG(o, o.pos);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // fades out along the beam, and fades in just past the lamp glass
                float along = pow(1.0 - i.along, 1.6) * smoothstep(0.0, 0.05, i.along);

                // the middle of the cone looks thicker than its edges
                float edge = pow(abs(dot(normalize(i.normal), normalize(i.view))), _EdgeSoftness);

                // soft where it meets the ground or walls, and gone right in front of the player
                float sceneDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screen)));
                float soft = saturate((sceneDepth - i.screen.z) / _SoftDistance);
                float near = saturate((i.screen.z - 1.0) / 4.0);

                float3 col = _Color.rgb * _Strength * along * edge * soft * near;
                UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0, 0, 0, 0));
                return fixed4(col, 1.0);
            }
            ENDCG
        }
    }
    Fallback Off
}
