///
/// Basic wireframe shader that can be used for rendering spatial mapping meshes.
///
Shader "GlitchShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }
    
    SubShader
    {
        Tags { "RenderType" = "Opaque" }

//        Pass
//        {
//            Cull Front
//            //Offset 50, 100
//
//            CGPROGRAM
//#pragma vertex vert
//#pragma fragment frag
//
//    // We only target the HoloLens (and the Unity editor), so take advantage of shader model 5.
//    //#pragma target 5.0
//    //#pragma only_renderers d3d11
//
//#include "UnityCG.cginc"
//
//        struct v2f
//        {
//            float4 viewPos : SV_POSITION;
//            UNITY_VERTEX_OUTPUT_STEREO
//        };
//
//        float3 _HitPoint;
//        float _FadeStartDist;
//        float _FadeMaxDist;
//        float _BloatAmount;
//
//        v2f vert(appdata_base v)
//        {
//            UNITY_SETUP_INSTANCE_ID(v);
//            v2f o;
//
//            float4 worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0));
//            float dist = length(worldPos - _HitPoint);
//            float bloatFactor = 1 - saturate((dist - _FadeStartDist) / (_FadeMaxDist - _FadeStartDist));
//            v.vertex.xyz += bloatFactor * _BloatAmount * v.normal;
//            o.viewPos = UnityObjectToClipPos(v.vertex);
//            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
//            return o;
//        }
//
//        float4 frag(v2f i) : COLOR
//        {
//            return float4(1,0,0,1);
//        }
//            ENDCG
//        }

        Pass
        {
        //Offset 50, 100
        Cull Back

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag

        // We only target the HoloLens (and the Unity editor), so take advantage of shader model 5.
        //#pragma target 5.0
        //#pragma only_renderers d3d11

        #include "UnityCG.cginc"

        struct v2f
        {
            float4 viewPos : SV_POSITION;
            UNITY_VERTEX_OUTPUT_STEREO
            float2 uv : TEXCOORD0;
            float3 worldPos : TEXCOORD1;
        };

        sampler2D _MainTex;

        float3 _HitPoint;
        float _FadeStartDist;
        float _FadeMaxDist;
        float2 _TexOffset;
        float _BloatAmount;

        v2f vert(appdata_base v)
        {
            UNITY_SETUP_INSTANCE_ID(v);
            v2f o;

            o.worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0));
            float dist = length(o.worldPos - _HitPoint);

            float bloatFactor = 1 - saturate((dist - _FadeStartDist) / (_FadeMaxDist - _FadeStartDist));
            v.vertex.xyz += bloatFactor * _BloatAmount * v.normal;
            o.viewPos = UnityObjectToClipPos(v.vertex);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
            o.uv = o.viewPos.xy / o.viewPos.w;
            o.uv = 0.5 * (o.uv + 1);
            return o;
        }

        float4 frag(v2f i) : COLOR
        {
            float dist3d = length(i.worldPos - _HitPoint);
            float dist2d = length(i.worldPos.xz - _HitPoint.xz);

            fixed4 glitchCol = tex2D(_MainTex, i.uv + _TexOffset);

            float4 col = glitchCol;
            if (i.worldPos.y >= _HitPoint.y)
            {
                col = lerp(col, float4(0, 0, 0, 1), saturate((dist3d - _FadeStartDist) / (_FadeMaxDist - _FadeStartDist)));
            }
            else
            {
                col = lerp(col, float4(0, 0, 0, 1), saturate((dist2d - _FadeStartDist) / (_FadeMaxDist - _FadeStartDist)));
            }
            return col;
        }
        ENDCG
    }
    }
        FallBack "Diffuse"
}
