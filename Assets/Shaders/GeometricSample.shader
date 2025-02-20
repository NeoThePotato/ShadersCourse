Shader "Unlit/GeometricSample"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Extruded ("Extruded", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag

            #include "C:\Program Files\Unity\2022.3.52f1\Editor\Data\CGIncludes\UnityCG.cginc" // TODO This is ugly, find a way to make relative path work

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal: NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2g
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct g2f
            {
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float _Extruded;

            v2g vert (appdata v)
            {
                v2g o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = v.normal;
                return o;
            }

            [maxvertexcount(6)]
            void geom (triangle v2g input[3], inout TriangleStream<g2f> triStream)
            {
                /*g2f output;
                float3 normal = normalize(cross(input[1].vertex - input[0].vertex, input[2].vertex - input[0].vertex));
                for (int i = 0; i < 3; i++)
                {
                    float4 vert = input[i].vertex;
                    vert.xyz += normal * (_Extruded);
                    output.vertex = UnityObjectToClipPos(vert);
                    output.uv = input[i].uv;
                    output.normal = UnityObjectToWorldNormal(normal);
                    output.worldPos = mul(UNITY_MATRIX_M, float4(input[i].vertex.xyz, 1));
                    triStream.Append(output);
                }
                triStream.RestartStrip();*/
                for (int i = 0; i < 3; i++)
                {
                    g2f output;
                    output.normal = input[i].normal;
                    output.uv = input[i].uv;
                    output.vertex = input[i].vertex;
                    triStream.Append(output);

                }
                triStream.RestartStrip();
            }

            fixed4 frag (g2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
