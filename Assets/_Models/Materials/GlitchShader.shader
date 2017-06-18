Shader "Hidden/NewImageEffectShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _DisruptTex("Disruptive Texture", 2D) = "white" {}
    }
        SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM
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
                float distance : TEXCOORD1;
            };

            struct PixelOutput
            {
                float4 color : SV_TARGET;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.distance = o.vertex.z;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _DisruptTex;

            PixelOutput frag(v2f i)
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 dis = tex2D(_DisruptTex, i.uv);
                float d = 0.50f; // saturate(i.distance);
                //col = lerp(col, dis, d);
                col.rgb = (1 - col).rgb;
                PixelOutput po;
                po.color = col;

                return po;
            }
            ENDHLSL
        }
    }
}
