// same as WorldTexture but the texture is stuck to the object instead of the world,
// so it moves with things that move (like trees falling over)
Shader "CART315/ObjectTexture"
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
        // no batching: unity merging the pieces would put them back in world space and the bark would slide again
        Tags { "RenderType" = "Opaque" "DisableBatching" = "True" }

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert
        #pragma target 3.5
        #include "UnityStandardUtils.cginc"

        sampler2D _MainTex, _BumpMap;
        fixed4 _Color;
        float _Scale, _BumpStrength, _Glossiness;

        struct Input
        {
            float3 objPos;
            float3 objNormal;
            float3 worldNormal;
            INTERNAL_DATA
        };

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            // object size so it still tiles by real metres on stretched cylinders
            float3 s = float3(length(unity_ObjectToWorld._m00_m10_m20),
                              length(unity_ObjectToWorld._m01_m11_m21),
                              length(unity_ObjectToWorld._m02_m12_m22));
            o.objPos = v.vertex.xyz * s;
            o.objNormal = normalize(v.normal / s);
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            o.Normal = float3(0, 0, 1);
            float3 n = normalize(IN.objNormal);

            float3 blend = pow(abs(n), 6.0);
            blend /= dot(blend, 1.0);

            float3 p = IN.objPos / _Scale;
            float2 uvX = p.zy, uvY = p.xz, uvZ = p.xy;

            fixed3 albedo = tex2D(_MainTex, uvX).rgb * blend.x
                          + tex2D(_MainTex, uvY).rgb * blend.y
                          + tex2D(_MainTex, uvZ).rgb * blend.z;

            float3 bumpX = UnpackScaleNormal(tex2D(_BumpMap, uvX), _BumpStrength);
            float3 bumpY = UnpackScaleNormal(tex2D(_BumpMap, uvY), _BumpStrength);
            float3 bumpZ = UnpackScaleNormal(tex2D(_BumpMap, uvZ), _BumpStrength);

            bumpX = float3(bumpX.xy + n.zy, abs(bumpX.z) * n.x);
            bumpY = float3(bumpY.xy + n.xz, abs(bumpY.z) * n.y);
            bumpZ = float3(bumpZ.xy + n.xy, abs(bumpZ.z) * n.z);
            float3 objBump = normalize(bumpX.zyx * blend.x + bumpY.xzy * blend.y + bumpZ.xyz * blend.z);
            float3 worldBump = normalize(UnityObjectToWorldNormal(objBump));

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
