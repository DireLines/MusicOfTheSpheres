Shader "Custom/Stipple" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _DegreesCutoff("Degrees Cutoff", float) = 30.0
        _StippleMultiplier("Stiple Multiplier", float) = 1.0
        _PlayerPosition ("Player Position", Vector) = (0.0, -1.35, 0.0, 1.0)
    }
    SubShader {
        LOD 200
        Tags { "RenderType" = "Opaque" }

        CGPROGRAM
        #pragma surface surf Lambert
    
        struct Input {
            float2 uv_MainTex;
            float4 screenPos;
            float3 localPos;
            float3 worldPos;
            float3 viewDir;
        };

        sampler2D _MainTex;
        float4 _Color;
        float4 _PlayerPosition;
        float _DegreesCutoff;
        float _StippleMultiplier;


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


        void surf (Input IN, inout SurfaceOutput o) {            
            // Screen-door transparency: Discard pixel if below threshold.
            // float4x4 thresholdMatrix =
            // {  1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
            // 13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
            // 4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
            // 16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
            // };
            
            // float pixelDist = distance(IN.worldPos, _WorldSpaceCameraPos);
            
            // float3 playerDirection = normalize(_WorldSpaceCameraPos - _PlayerPosition);
            // float pixelDirection = normalize(IN.worldPos - _WorldSpaceCameraPos);
            // float angle = acos(dot(playerDirection, pixelDirection) / (playerDirection * pixelDirection));
            // // float angleDegrees = angle * 180.0 / 3.1415926535;
            // float angleDegrees = angle;
            // float angleStep = step(_DegreesCutoff, angleDegrees);

            // float angleDist = (angleDegrees - _DegreesCutoff) * + _StippleMultiplier;
            // angleDist = lerp(angleDist, 99999999, angleStep);
            
            // float dist = min(pixelDist, angleDist);

            // float4x4 _RowAccess = { 1,0,0,0, 0,1,0,0, 0,0,1,0, 0,0,0,1 };
            // float2 pos = IN.screenPos.xy / IN.screenPos.w;
            // pos *= _ScreenParams.xy; // pixel position
            // clip(clamp(dist - _ProjectionParams.y - 1, 0, 1) - thresholdMatrix[fmod(pos.x, 4)] * _RowAccess[fmod(pos.y, 4)]);
            float4 hehe = (1.0, 0.0, 0.0, 1.0);
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * hehe;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"

}           