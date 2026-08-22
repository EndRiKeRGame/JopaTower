Shader "Custom/URP/GrabPointHighlight"
{
    // Подсветка точек захвата: контур по краю спрайта + пульсирующее свечение.
    // Работает как обычный спрайт-шейдер (Blend SrcAlpha OneMinusSrcAlpha),
    // поэтому просто заменяет Sprites/Default на материале GrabPoint.

    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        [HDR] _OutlineColor ("Outline / Glow Color", Color) = (1, 0.85, 0.1, 1) // золотистый
        _OutlineWidth ("Outline Width (px)", Range(0, 10)) = 2

        _PulseSpeed ("Pulse Speed", Float) = 3
        _PulseIntensity ("Pulse Intensity", Range(0, 1)) = 0.6

        // Управляется из скрипта: 0 = подсветка выключена, 1 = включена.
        // Через MaterialPropertyBlock, чтобы не плодить инстансы материала на каждую точку.
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
            Name "GrabPointHighlight"

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

                // Контур: смотрим альфу соседних пикселей — если рядом "воздух" (alpha ~0),
                // а тут спрайт уже прозрачный по краю — рисуем контур цветом _OutlineColor.
                float2 texel = _MainTex_TexelSize.xy * _OutlineWidth;
                half neighborAlpha = 0;
                neighborAlpha += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2( texel.x, 0)).a;
                neighborAlpha += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2(-texel.x, 0)).a;
                neighborAlpha += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2(0,  texel.y)).a;
                neighborAlpha += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2(0, -texel.y)).a;
                neighborAlpha = saturate(neighborAlpha);

                float pulse = (sin(_Time.y * _PulseSpeed) * 0.5 + 0.5) * _PulseIntensity;

                // Контур виден только там, где сам пиксель прозрачный, а сосед — нет
                half outlineMask = neighborAlpha * (1 - baseColor.a) * _HighlightAmount;
                half4 outline = _OutlineColor;
                outline.a *= outlineMask;

                // Внутреннее пульсирующее свечение поверх самого спрайта
                half4 glow = _OutlineColor * (pulse * _HighlightAmount * baseColor.a);

                half4 result = baseColor;
                result.rgb += glow.rgb;
                result = lerp(result, outline, outline.a);

                return result;
            }
            ENDHLSL
        }
    }

    FallBack "Sprite/Unlit"
}
