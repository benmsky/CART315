// 3D grass clumps. Sways gently, gets darker at the roots,
// and shrinks away in the distance where the fog would hide it anyway.
Shader "CART315/Grass3D"
{
    Properties
    {
        _MainTex ("Blades", 2D) = "white" {}
        _Cutoff ("Cutoff", Range(0, 1)) = 0.4
        _Brightness ("Brightness", Float) = 2.2
        _Wind ("Wind Strength", Float) = 0.05
        _FadeStart ("Shrink Starts At", Float) = 30
        _FadeEnd ("Gone By", Float) = 45
    }

    SubShader
    {
        Tags { "Queue" = "AlphaTest" "RenderType" = "TransparentCutout" "IgnoreProjector" = "True" }
        Cull Off

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert alphatest:_Cutoff fullforwardshadows nolightmap
        #pragma target 3.0

        sampler2D _MainTex;
        float _Wind, _FadeStart, _FadeEnd, _Brightness;

        struct Input
        {
            float2 uv_MainTex;
            float3 color;
        };

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            float3 world = mul(unity_ObjectToWorld, v.vertex).xyz;

            float top = v.texcoord.y;
            float sway = sin(_Time.y * 1.4 + world.x * 0.45 + world.z * 0.3);
            v.vertex.x += sway * _Wind * top * top;

            float distanceAway = distance(world, _WorldSpaceCameraPos);
            float size = 1.0 - smoothstep(_FadeStart, _FadeEnd, distanceAway);
            v.vertex.y *= size;

            o.color = v.color.rgb * lerp(0.35, 1.0, top);
        }

        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed4 blades = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = saturate(blades.rgb * IN.color * _Brightness);
            o.Alpha = blades.a;
        }
        ENDCG
    }
    Fallback "Transparent/Cutout/VertexLit"
}
