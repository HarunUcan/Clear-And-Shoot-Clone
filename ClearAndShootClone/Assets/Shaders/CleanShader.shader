Shader "Unlit/CleanShader"
{
   Properties
    {
        _MainTex ("Dirty Texture", 2D) = "white" {}   // kirli kaplama
        _CleanTex ("Clean Texture", 2D) = "white" {}  // temiz kaplama
        _MaskTex ("Mask Texture", 2D) = "white" {}    // mask rendertexture
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            sampler2D _CleanTex;
            sampler2D _MaskTex;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 dirty = tex2D(_MainTex, i.uv);
                float4 clean = tex2D(_CleanTex, i.uv);
                float mask = tex2D(_MaskTex, i.uv).r;

                // mask = 1 -> kirli, 0 -> temiz
                return lerp(clean, dirty, mask);
            }
            ENDCG
        }
    }
}
