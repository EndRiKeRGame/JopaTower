Shader "Custom/URP/StandablePlatformHighlight"
{
    // Подсветка платформ/зон, на которых можно стоять:
    // обычный контур по краю спрайта + отдельная светящаяся полоса вдоль ВЕРХНЕЙ грани
    // (именно верх — это та поверхность, на которую персонаж встаёт).

    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        [HDR] _OutlineColor ("Outline Color", Color) = (0.2, 1, 0.4, 1) // зелёный
        _OutlineWidth ("Outline Width (px)", Range(0, 10)) = 2

        [HDR] _TopEdgeColor ("Top Edge (Standable Surface) Color", Color) = (0.4, 1, 0.6, 1)
        _TopEdgeThickness ("Top Edge Thickness (0..1 of sprite height)", Range(0, 0.5)) = 0.08

        _PulseSpeed ("Pulse Speed", Float) = 2.5
        _PulseIntensity ("Pulse Intensity", Range(0, 1)) = 0.5

        // Управляется из скрипта: 0 = подсветка выключена, 1 = включена.
        _HighlightAmount ("Highlight Amount", Range(0, 1)) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        ZWrite Off
        Lighting Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "StandablePlatformHighlight"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;

            half4 _Color;
            half4 _OutlineColor;
            float _OutlineWidth;

            half4 _TopEdgeColor;
            float _TopEdgeThickness;

            float _PulseSpeed;
            float _PulseIntensity;
            float _HighlightAmount;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color = IN.color * _Color;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 baseColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * IN.color;

                float pulse = (sin(_Time.y * _PulseSpeed) * 0.5 + 0.5) * _PulseIntensity;

                // --- Обычный контур по краю спрайта (как у точек захвата) ---
                float2 texel = _MainTex_TexelSize.xy * _OutlineWidth;
                half neighborAlpha = 0;
                neighborAlpha += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2( texel.x, 0)).a;
                neighborAlpha += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2(-texel.x, 0)).a;
                neighborAlpha += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2(0,  texel.y)).a;
                neighborAlpha += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2(0, -texel.y)).a;
                neighborAlpha = saturate(neighborAlpha);

                half outlineMask = neighborAlpha * (1 - baseColor.a) * _HighlightAmount;
                half4 outline = _OutlineColor;
                outline.a *= outlineMask;

                // --- Акцент на верхней грани: полоса высотой _TopEdgeThickness от uv.y == 1 ---
                float distanceFromTop = 1.0 - IN.uv.y; // 0 на самом верху, 1 внизу
                float topMask = 1.0 - smoothstep(0.0, _TopEdgeThickness, distanceFromTop);
                topMask *= baseColor.a * _HighlightAmount; // подсвечиваем только там, где сам спрайт непрозрачен

                half4 topGlow = _TopEdgeColor * (topMask * (0.6 + pulse));

                half4 result = baseColor;
                result.rgb += topGlow.rgb;
                result = lerp(result, outline, outline.a);

                return result;
            }
            ENDHLSL
        }
    }

    FallBack "Sprite/Unlit"
}
