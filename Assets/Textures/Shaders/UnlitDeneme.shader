Shader "Custom/UnlitTransparentScrollingTextureShader" {
    Properties {
        _MainTex("Texture", 2D) = "white" {}
        _Speed("Speed", Range(-10, 10)) = 1
        _TintColor("Tint Color", Color) = (1, 1, 1, 1)
    }

    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _Speed;
            float4 _TintColor;

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                float2 scrollingUV = float2(i.uv.x + (_Time.y * _Speed), i.uv.y);
                float4 texColor = tex2D(_MainTex, scrollingUV);
                float4 col = texColor * _TintColor;
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
