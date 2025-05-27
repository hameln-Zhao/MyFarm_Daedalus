Shader "Custom/PixelFishingLine"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Base Color", Color) = (1,1,1,1)
        _PixelSnap ("Pixel Snap", Range(0, 1)) = 1
        _LineWidth ("Line Width", Range(0.01, 0.5)) = 0.1
        _WaveIntensity ("Wave Intensity", Range(0, 0.5)) = 0.1
        _WaveSpeed ("Wave Speed", Float) = 2.0
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
        }
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma multi_compile _ WAVE_ON
            
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float waveFactor : TEXCOORD1;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _PixelSnap;
            float _LineWidth;
            float _WaveIntensity;
            float _WaveSpeed;
            
            v2f vert (appdata v)
            {
                v2f o;
                
                // 基础顶点变换
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                
                // 波浪效果
                #ifdef WAVE_ON
                float wave = sin(_Time.y * _WaveSpeed + worldPos.x * 10.0) * _WaveIntensity;
                worldPos.y += wave * (1.0 - v.uv.x); // 波浪从鱼竿向鱼漂递减
                #endif
                
                o.vertex = mul(UNITY_MATRIX_VP, worldPos);
                
                // 像素对齐
                #ifdef PIXELSNAP_ON
                if (_PixelSnap > 0)
                {
                    o.vertex = UnityPixelSnap(o.vertex);
                }
                #endif
                
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;
                
                // 传递波浪因子给片段着色器
                o.waveFactor = saturate(abs(v.uv.x - 0.5) * 2.0); // 中心为0，边缘为1
                
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // 像素化处理
                float2 pixelUV = floor(i.uv * _PixelSnap * 100) / (_PixelSnap * 100);
                
                // 计算线宽
                float distanceFromCenter = abs(i.uv.y - 0.5) * 2.0; // 0=中心, 1=边缘
                float lineWidth = _LineWidth;
                
                // 边缘抗锯齿
                float alpha = 1.0 - smoothstep(lineWidth - 0.1, lineWidth, distanceFromCenter);
                
                // 应用颜色
                fixed4 col = tex2D(_MainTex, pixelUV) * i.color;
                col.a *= alpha;
                
                // 添加波浪带来的透明度变化
                #ifdef WAVE_ON
                col.a *= 1.0 - (i.waveFactor * _WaveIntensity * 0.5);
                #endif
                
                return col;
            }
            ENDCG
        }
    }
    
    CustomEditor "PixelLineShaderEditor"
}