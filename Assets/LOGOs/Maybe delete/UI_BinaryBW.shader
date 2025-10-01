Shader "UI/BinaryBW"
{
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _Threshold("Threshold", Range(0,1)) = 0.50
        _Smooth("Edge Smooth", Range(0,0.2)) = 0.02
    }
    SubShader{
        Tags{ "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "CanUseSpriteAtlas"="True" }
        ZWrite Off
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass{
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex; float4 _MainTex_ST;
            float _Threshold, _Smooth;

            struct appdata { float4 vertex:POSITION; float2 uv:TEXCOORD0; float4 color:COLOR; };
            struct v2f { float4 pos:SV_POSITION; float2 uv:TEXCOORD0; float4 color:COLOR; };

            v2f vert(appdata v){ v2f o; o.pos = UnityObjectToClipPos(v.vertex); o.uv = TRANSFORM_TEX(v.uv,_MainTex); o.color = v.color; return o; }

            fixed4 frag(v2f i):SV_Target{
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;              // RawImage tint multiplies here
                float lum = dot(col.rgb, float3(0.299, 0.587, 0.114));      // grayscale luminance
                float t = smoothstep(_Threshold - _Smooth, _Threshold + _Smooth, lum);
                col.rgb = t.xxx;                                            // binary black/white
                return col;
            }
            ENDHLSL
        }
    }
}
