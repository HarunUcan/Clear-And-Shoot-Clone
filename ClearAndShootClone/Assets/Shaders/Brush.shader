Shader "Unlit/Brush"
{
    Properties
    {
        _MainTex ("Mask Texture", 2D) = "white" {}
        _BrushTex ("Brush Texture", 2D) = "white" {}  // optional
        _BrushPos ("Brush Position", Vector) = (0,0,0,0)
        _BrushSize ("Brush Size", Float) = 0.1
    }

    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            sampler2D _BrushTex;
            float4 _BrushPos;
            float _BrushSize;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float mask = tex2D(_MainTex, i.uv).r;

                // Mesafe hesapla
                float dist = distance(i.uv, _BrushPos.xy);

                // Eðer fýrça alanýndaysa mask deðerini azalt
                if(dist < _BrushSize)
                {
                    mask = saturate(mask - 0.5); // 0 = temiz, 1 = kirli
                }

                return float4(mask, mask, mask, 1);
            }
            ENDCG
        }
    }
}
