Shader "Custom/FOVShader"
{
    Properties
    {
        [MainColor] _BaseColor ("Base Color", Color) = (1, 1, 1, 0.3)
        _ChaseColor ("Chase Color", Color) = (1, 0, 0, 0.6)
        _FillAmount ("Fill Amount", Range(0, 1)) = 0
        _ViewDistance ("View Distance", Float) = 5
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent" 
            "Queue" = "Transparent" 
            "RenderPipeline" = "UniversalPipeline" 
        }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Name "ForwardLit"
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float3 positionOS   : TEXCOORD1;
                float2 uv           : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _BaseColor;
                float4 _ChaseColor;
                float _FillAmount;
                float _ViewDistance;
            CBUFFER_END

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.positionOS = IN.positionOS.xyz;
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float dist = length(IN.positionOS.xy);
                
                // We use the raw distance compared to the fill distance
                float fillDist = _FillAmount * _ViewDistance;
                
                // Use a small smoothstep for a nicer edge
                float isFilled = smoothstep(fillDist + 0.05, fillDist - 0.05, dist);
                
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                half4 finalColor = lerp(_BaseColor, _ChaseColor, isFilled);
                
                return col * finalColor;
            }
            ENDHLSL
        }
    }
}
