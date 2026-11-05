// A soft patch of fog for the particle systems in the woods.
// It fades where it touches the ground or trees, and fades away as the player walks through it.
Shader "CART315/FogPuff"
{
    Properties
    {
        _MainTex ("Puff Shape", 2D) = "white" {}
        _Opacity ("Opacity", Range(0, 1)) = 0.1
        _FogTint ("Fog Colour", Color) = (0.16, 0.21, 0.33, 1)
        _MoonTint ("Moonlit Colour", Color) = (0.30, 0.48, 0.90, 1)
        _MoonDir ("Moon Direction", Vector) = (-0.243, 0.342, -0.908, 0)
        _SoftDistance ("Soft Edge Distance", Float) = 2.5
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
            float _Opacity, _SoftDistance;
            fixed4 _FogTint, _MoonTint;
            float3 _MoonDir;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
                float4 screen : TEXCOORD1;
                float3 view : TEXCOORD2;
                UNITY_FOG_COORDS(3)
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                o.screen = ComputeScreenPos(o.pos);
                COMPUTE_EYEDEPTH(o.screen.z);
                o.view = mul(unity_ObjectToWorld, v.vertex).xyz - _WorldSpaceCameraPos;
                UNITY_TRANSFER_FOG(o, o.pos);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float alpha = tex2D(_MainTex, i.uv).r * i.color.a * _Opacity;

                // melts into whatever is behind it instead of cutting a hard line
                float sceneDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screen)));
                alpha *= saturate((sceneDepth - i.screen.z) / _SoftDistance);

                // fades away right in front of the player
                alpha *= saturate((i.screen.z - 1.0) / 4.0);

                // fog between the player and the moon lights up
                float towardMoon = saturate(dot(normalize(i.view), normalize(_MoonDir)));
                float3 col = lerp(_FogTint.rgb, _MoonTint.rgb, pow(towardMoon, 6.0));

                UNITY_APPLY_FOG(i.fogCoord, col);
                return fixed4(col, alpha);
            }
            ENDCG
        }
    }
    Fallback Off
}
