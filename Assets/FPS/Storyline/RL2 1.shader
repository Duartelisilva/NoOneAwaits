Shader "UI/RedLinesDistortedStronger3"
{
    Properties
    {
        _Speed ("Scroll Speed", Float) = 1.2
        _LineWidth ("Line Width", Float) = 0.002
        _LineSpacing ("Spacing", Float) = 0.1
        _LineColor ("Line Color", Color) = (1, 0.2, 0.2, 0.7)  // Brighter, thinner lines
        _FlickerSpeed ("Flicker Speed", Float) = 25.0
        _GlitchChance ("Glitch Chance", Range(0,1)) = 0.3
        _Distortion ("Distortion Strength", Float) = 0.03
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
            float _FlickerSpeed;
            float _GlitchChance;
            float _Distortion;

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // Hash function for randomness
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

                float2 pos100 = floor(i.uv * 100.0 + float2(time,0));

                // Horizontal distortion shifting UV.x by a small random offset
                float distortion = (hash21(pos100) - 0.5) * _Distortion;
                float distortedX = i.uv.x + distortion;

                float lineVal = frac(distortedX * 100.0 + time);
                float mask = smoothstep(0.0, _LineWidth, lineVal) * (1.0 - smoothstep(_LineWidth, _LineWidth * 2.0, lineVal));

                // Flicker with sine wave plus random noise
                float flickerBase = sin(_Time.y * _FlickerSpeed + i.uv.y * 50.0) * 0.6 + 0.4;
                float noiseFlicker = hash21(pos100 + float2(0,_Time.y * 20));
                float flicker = lerp(flickerBase, noiseFlicker, 0.3);

                // Random glitches: lines occasionally disappear
                float glitchSeed = hash21(pos100 + float2(_Time.y * 10, 0));
                if (glitchSeed < _GlitchChance)
                    mask = 0;

                fixed4 col = _LineColor * mask * flicker;

                return col;
            }
            ENDCG
        }
    }
}
