Shader "Custom/TransparentScrollingTextureShader" {
    Properties {
        _MainTex("Texture", 2D) = "white" {}
        _Speed("Speed", Range(-10, 10)) = 1
        _TintColor("Tint Color", Color) = (1, 1, 1, 1)
    }
 
    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;
        float _Speed;
        float4 _TintColor;

        struct Input {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutput o) {
            float2 scrollingUV = float2(IN.uv_MainTex.x + (_Time.y * _Speed), IN.uv_MainTex.y);
            float4 texColor = tex2D(_MainTex, scrollingUV);
            o.Albedo = texColor.rgb * _TintColor.rgb;
            o.Alpha = texColor.a * _TintColor.a;
            o.Specular = 0;
            o.Gloss = 0;
            o.Emission = texColor.rgb * (1 - texColor.a); // Ekleme: Emission değerini ayarlayın
        }
        ENDCG
    }
    FallBack "Diffuse"
}
