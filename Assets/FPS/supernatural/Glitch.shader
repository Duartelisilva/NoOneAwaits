Shader "UI/UnderwaterEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TimeSpeed ("Time Speed", Float) = 1.0
        _DistortionStrength ("Distortion Strength", Range(0,0.05)) = 0.02
        _ColorTint ("Color Tint", Color) = (0.0, 0.3, 0.5, 0.5)
        _PlayerY ("Player Y Position (0-1 UV space)", Float) = 0.0
        _FadeDistance ("Fade Distance", Float) = 0.2
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _TimeSpeed;
            float _DistortionStrength;
            float4 _ColorTint;
            float _PlayerY;
            float _FadeDistance;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float localY : TEXCOORD1;
            };

            float wave(float2 uv, float time)
            {
                return sin(uv.x * 40.0 + time) * 0.003 + cos(uv.y * 30.0 + time * 1.5) * 0.003;
            }

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.localY = v.uv.y; // use UV.y for vertical position in UI
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float time = _Time.y * _TimeSpeed;

                float2 distortion = float2(wave(i.uv, time), wave(i.uv, time * 1.3));
                float2 uvDistorted = i.uv + distortion * _DistortionStrength;

                fixed4 col = tex2D(_MainTex, uvDistorted);

                // Fade alpha based on UV.y vs _PlayerY and _FadeDistance
                float fadeStart = _PlayerY;
                float fadeEnd = _PlayerY + _FadeDistance;
                float alphaFade = saturate((fadeEnd - i.localY) / _FadeDistance);

                if (i.localY > fadeEnd)
                    discard;

                col.rgb = lerp(col.rgb, _ColorTint.rgb, _ColorTint.a);
                col.a *= alphaFade;

                return col;
            }
            ENDCG
        }
    }
}
