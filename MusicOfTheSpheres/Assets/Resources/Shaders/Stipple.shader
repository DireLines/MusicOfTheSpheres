Shader "Custom/Stipple" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _DegreesCutoff("Degrees Cutoff", float) = 30.0
    }
    SubShader {
        Blend SrcAlpha OneMinusSrcAlpha
        Tags { "RenderType"="Opaque" }
        LOD 200
        Cull Off

        // Stencil {
        //     Ref 1
        //     Comp NotEqual
        // }

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
        float _DegreesCutoff;

        float3 _PlayerPosition; //Global Vector

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutput o) {           
            
            //Color setting
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
            
            float3 pointDir = normalize(IN.worldPos - _WorldSpaceCameraPos);
            float3 pointDist = distance(IN.worldPos, _WorldSpaceCameraPos);
            float3 playerDir = normalize(_PlayerPosition - _WorldSpaceCameraPos);
            float3 playerDist = distance(_PlayerPosition, _WorldSpaceCameraPos);

            float cosAngle = dot(playerDir, pointDir);
            float angle = acos(cosAngle);
            float angleDegrees = angle * 180.0 / 3.14159;
            float angleMask = step(angleDegrees, _DegreesCutoff * 9.0 / playerDist);
            float floorMask = step(_PlayerPosition.y, IN.worldPos.y);
            float distMask = step(pointDist, playerDist);
            float mask = 1 - (angleMask * floorMask * distMask);
            
            // Screen-door transparency: Discard pixel if below threshold.
            float4x4 thresholdMatrix =
            {  1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
            13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
            4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
            16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
            };
            
            float dist = mask * distance(IN.worldPos, _WorldSpaceCameraPos);
            
            dist = (mask) * 10;

            float4x4 _RowAccess = { 1,0,0,0, 0,1,0,0, 0,0,1,0, 0,0,0,1 };
            float2 pos = IN.screenPos.xy / IN.screenPos.w;
            pos *= _ScreenParams.xy; // pixel position
            clip(clamp(dist - _ProjectionParams.y - 1, 0, 1) - thresholdMatrix[fmod(pos.x, 4)] * _RowAccess[fmod(pos.y, 4)]);
        }
        ENDCG
    }
    FallBack "Diffuse"

}           