Shader "UI/RedLinesDistortedStronger"
{
    Properties
    {
        _Speed ("Scroll Speed", Float) = 0.35
        _LineWidth ("Line Width", Float) = 0.006
        _LineSpacing ("Spacing", Float) = 0.4
        _LineColor ("Line Color", Color) = (1, 0, 0, 0.6)  // More opaque
        _Distortion ("Distortion Strength", Float) = 0.02
        _FlickerSpeed ("Flicker Speed", Float) = 8.0
        _GlitchChance ("Glitch Chance", Range(0,1)) = 0.06
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float _Speed;
            float _LineWidth;
            float _LineSpacing;
            float4 _LineColor;
            float _Distortion;
            float _FlickerSpeed;
            float _GlitchChance;

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 345.45));
                p += dot(p, p + 34.345);
                return frac(p.x * p.y);
            }

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float time = _Time.y * _Speed;

                float2 pos100 = floor(i.uv * 50.0 + time);

                float distortion = (hash21(pos100) - 0.5) * _Distortion;

                float uvOffset = i.uv.x + i.uv.y + time + distortion;

                float lineVal = frac(uvOffset / _LineSpacing);
                float mask = smoothstep(0.0, _LineWidth, lineVal) * (1.0 - smoothstep(_LineWidth, _LineWidth * 2.0, lineVal));

                float flicker = (sin(_Time.y * _FlickerSpeed + uvOffset * 40.0) * 0.5 + 0.5); // stronger flicker

                float glitchSeed = hash21(pos100 + float2(_Time.y * 5, 0));
                if (glitchSeed < _GlitchChance)
                    flicker *= step(0.5, frac(_Time.y * 10));

                float lineBreakSeed = hash21(pos100 + float2(0, _Time.y * 7));
                if (lineBreakSeed < _GlitchChance * 0.3)
                    mask = 0;

                float pulse = 0.75 + 0.4 * sin(_Time.y * 3.5);  // stronger, faster pulse

                fixed4 col = _LineColor * mask * flicker * pulse;

                float darkness = smoothstep(0.0, 0.05, mask);
                col.a *= darkness;

                return col;
            }
            ENDCG
        }
    }
}
