Shader "Custom/ScrollingTextureShader" {
    Properties {
        _MainTex("Texture", 2D) = "white" {}
        _Speed("Speed", Range(-10, 10)) = 1
    }
 
    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert
        sampler2D _MainTex;
        float _Speed;

        struct Input {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutput o) {
            float2 scrollingUV = float2(IN.uv_MainTex.x + (_Time.y * _Speed), IN.uv_MainTex.y);
            o.Albedo = tex2D(_MainTex, scrollingUV).rgb;
            o.Alpha = tex2D(_MainTex, scrollingUV).a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
