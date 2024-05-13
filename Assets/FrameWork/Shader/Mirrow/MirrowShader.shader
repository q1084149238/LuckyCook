//只是个贴图shader而已
Shader "Custom/MirrowShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Always" }

        pass
        {
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment Pixel
            #include "UnityCG.cginc"

            sampler2D _MainTex;

            struct vertexOutput
            {
                float4 pos: SV_POSITION;
                float3 uv: TEXCOORD0;
            };

            vertexOutput Vertex(appdata_base i)
            {
                vertexOutput o;
                o.pos = UnityObjectToClipPos(i.vertex);
                o.uv = i.texcoord;

                return o;
            }

            float4 Pixel(vertexOutput i): SV_TARGET
            {
                return tex2D(_MainTex, i.uv);
            }

            ENDCG
        }  
    }
    FallBack "Diffuse"
}
