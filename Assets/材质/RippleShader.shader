Shader "Custom/RippleShader"
{
    Properties
    {
        _Color ("Ripple Color", Color) = (0.5, 0.7, 1, 0.8)
        _RippleStrength ("Ripple Strength", Range(0, 2)) = 1.0
        _RippleSpeed ("Ripple Speed", Range(0, 10)) = 2.0
        _RippleFrequency ("Ripple Frequency", Range(10, 50)) = 30.0
        _RippleSpread ("Ripple Spread", Range(0, 5)) = 1.0
        _FadeOut ("Fade Out", Range(0, 3)) = 1.5
        _MaxRadius ("Max Radius", Range(0, 1)) = 1.0
        _EdgeFade ("Edge Fade", Range(0, 0.5)) = 0.2
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float2 centerUV : TEXCOORD1;
            };
            
            float4 _Color;
            float _RippleStrength;
            float _RippleSpeed;
            float _RippleFrequency;
            float _RippleSpread;
            float _FadeOut;
            float _StartTime;
            float _MaxRadius;
            float _EdgeFade;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                
                float2 centerToUV = v.uv - 0.5;
                o.centerUV = centerToUV;
                
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float dist = length(i.centerUV * 2);
                float t = _Time.y - _StartTime;
                float wave = dist - _RippleSpread * t;
                
                // 计算到UV边缘的距离（用于边缘渐隐）
                float distToEdge = min(min(i.uv.x, 1.0 - i.uv.x), min(i.uv.y, 1.0 - i.uv.y));
                
                float ripple = abs(sin(wave * _RippleFrequency));
                ripple = smoothstep(0.85, 1.0, ripple);
                
                float ripple2 = abs(sin(wave * _RippleFrequency * 1.15));
                ripple2 = smoothstep(0.75, 0.95, ripple2);
                ripple += ripple2 * 0.4;
                
                float attenuation = 1.0 / (1.0 + dist * 5.0);
                float fadeOut = 1.0 - saturate(t / _FadeOut);
                
                // 范围限制：超出最大半径则完全透明
                float radiusMask = step(dist, _MaxRadius);
                
                // 边缘渐隐效果
                float edgeFade = smoothstep(0, _EdgeFade, distToEdge);
                
                float alpha = ripple * attenuation * fadeOut * _RippleStrength * radiusMask * edgeFade;
                
                if (alpha < 0.02)
                {
                    discard;
                }
                
                fixed4 col = _Color;
                col.a = alpha;
                
                return col;
            }
            ENDCG
        }
    }
    
    FallBack "Diffuse"
}
