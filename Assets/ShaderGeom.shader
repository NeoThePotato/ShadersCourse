Shader "Geometric/GeometricBase" // shader name  - shader1 under shader category - Unlit
{
    // properties to be expose in the material
    Properties
    {
        // Color Property: Used for specifying color values.
        _Color("Main Color", Color) = (1, 1, 1)
        // Color Property with Alpha: Used for specifying color values with an alpha channel.
        _ColorRGBA("My Color with Alpha", Color) = (1, 1, 1, 1)
        // Float Property: Used for specifying single floating-point values.
        _Extruded("Extruded", Range(0, 1)) = 0.5 //with a specified range.
        // Vector Property: Used for specifying vector values (e.g., Vector2, Vector3, Vector4).
        _Vector("My Vector", Vector) = (1, 0, 0, 1)
        // 2D Texture Property: Used for specifying 2D texture maps.
        _MainTex("Main Texture", 2D) = "white" { }
        // Cube Map Property: Used for specifying cube maps.
        _Cube("My Cube Map", CUBE) = "" { }
        // Render Texture Property: Used for specifying render textures.
        _RenderTexture("My Render Texture", 2D) = "" { }
    }
    // specific shader (there might be more then on subshader) under one defined shader.
    SubShader
    {
        Tags 
        {
             "RenderType"="Opaque"
             "Queue"="Transparent" 
        } 
        
        LOD 100

        
        //blending parameters
        // SrcFactor is this color DesFactor is the color that is been render before at this vert position
        // syntax: Blend SrcFactor DstFactor
        // FinalColor = (SourceColor × SrcFactor) + (DestinationColor × DstFactor)
        
        Cull Off
        ZWrite Off
        Blend One One // additive

        Pass
        {
            
            // order of the code:
            // assume this is like C,
            // so all structures,functions, var - need to be place BEFORE using them!
            
            // variables precision:
            //float    32-bit floating point.
            //half     16-bit floating point.
            //int      32-bit integer.
            //bool     Boolean (true/false).
            
            
            
            
            CGPROGRAM
            #pragma vertex vert // define the name of the vertex func
            #pragma geometry geom
            #pragma fragment frag // define the name of the fragment func
            
            #include "C:\Program Files\Unity\2022.3.52f1\Editor\Data\CGIncludes\UnityCG.cginc" // TODO This is ugly, find a way to make relative path work

            // Common Semantics
            // POSITION: Vertex position (object space or clip space).
            // NORMAL: Vertex normal (object space).
            // TEXCOORD0, TEXCOORD1, ...: Texture coordinates.
            // COLOR: Vertex color.
            // SV_Position: Special system value for clip space position.
            // SV_Target: Output color for the fragment shader.
            

            
            // structure to be pass from the app to the vertex
            struct appdata
            {
                // common attribute container naming for per-vertex that can be pass to vert function
                float4 vertex : POSITION; // position in object space
                float3 normal : NORMAL; // normals in object space
                float2 uv : TEXCOORD0; // the first (num 0 ) texture coordinates in the mat
                
                //more attributes
                float4 tang: TANGENT; // tangent direction (xyz) tangent sign (w)
                float4 color : COLOR; 
            };

            struct v2g
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float3 worldPos: TEXCOORD1;
            };
            
            struct g2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float3 worldPos: TEXCOORD1;
            };

            
            // declare shaders variables
            // syntax: <PropertyType> <VariableName>;
            sampler2D _MainTex; 
            float4 _MainTex_ST; // Stores tiling (x, y) and offset (z, w).
            float _Extruded;
            float4 _Color;

            // vertex func - first function in the flow
            v2g vert (appdata v)
            {
                v2g o;
                //o.vertex = UnityObjectToClipPos(v.vertex); // from object directly to clip (camera) pos
                o.vertex = v.vertex;
                //o.normal = UnityObjectToWorldNormal(v.normal);
                o.normal = v.normal;
                o.worldPos = mul(UNITY_MATRIX_M, float4(v.vertex.xyz,1)); // from object to world pos
                o.uv = v.uv;
                return o;
            }


            [maxvertexcount(6)]
            void geom(triangle v2g input[3], inout TriangleStream<g2f> triStream)
            {
                
                g2f o;
                //recalculate the normal
                float3 normal = normalize(cross(input[1].vertex - input[0].vertex, input[2].vertex - input[0].vertex));
                
                //draw the normal triangles, but extruded outwards
                for(int i = 0; i < 3; i++)
                {
                    float4 vert = input[i].vertex;
                    vert.xyz += normal * (_Extruded);  //math to move tri's out
                    o.vertex = UnityObjectToClipPos(vert);
                    o.uv = input[i].uv;
                    o.normal = UnityObjectToWorldNormal(normal);
                    o.worldPos = mul(UNITY_MATRIX_M, float4(input[i].vertex.xyz,1));
                    triStream.Append(o);
                }
                triStream.RestartStrip();
                
                //process verts in reverse order, creating a backwards tri
                /*
                for(int i = 2; i >= 0; i--)
                {
                    float4 vert = input[i].vertex;
                    vert.xyz += normal * (_Extruded);  //math to move tri's out
                    o.vertex = UnityObjectToClipPos(vert);
                    o.uv = input[i].uv;
                    //invert normal
                    o.normal = UnityObjectToWorldNormal(-1*normal);
                    o.worldPos = mul(UNITY_MATRIX_M, float4(input[i].vertex.xyz,1));
                    triStream.Append(o);
                }
                triStream.RestartStrip();
                */

                 // Geometry Shader: Generates a quad from each triangle
            
                // Create a quad from the triangle
                /*
                float3 offset = float3(0.1, 0.1, 0.0);  // Arbitrary offset for the quad vertices
                for (int i = 0; i < 3; i++)
                {
                    o.uv = input[i].uv;
                    o.normal = UnityObjectToWorldNormal(input[i].normal);
                    o.vertex = UnityObjectToClipPos(input[i].vertex + offset); // Offset each vertex
                    o.worldPos = mul(UNITY_MATRIX_M, float4(input[i].vertex.xyz,1));
                    triStream.Append(o);
                }

                // Also create a fourth vertex to form the quad
                g2f fourthVert;
                fourthVert.vertex = UnityObjectToClipPos(input[0].vertex + offset * 2); // Another offset
                fourthVert.normal = UnityObjectToWorldNormal(input[0].normal);
                fourthVert.uv = input[0].uv;
                fourthVert.worldPos = mul(UNITY_MATRIX_M, float4(input[0].vertex.xyz,1));
                triStream.Append(fourthVert);

                triStream.RestartStrip(); // Close the triangle strip
                */
                
                float3 center = (input[0].vertex + input[1].vertex + input[2].vertex) / 3.0;
                float3 combinedNormal = normalize(input[0].normal + input[1].normal + input[2].normal);
                float3 newVertex = center + combinedNormal * _Extruded;
                 for (int i = 0; i < 3; i++)
                {
                    o.uv = input[i].uv;
                    o.normal = UnityObjectToWorldNormal(input[i].normal);
                    o.vertex = UnityObjectToClipPos(input[i].vertex); // Offset each vertex
                    o.worldPos = mul(UNITY_MATRIX_M, float4(input[i].vertex.xyz,1));
                    triStream.Append(o);
                }
                
                o.uv = input[0].uv;
                o.normal = UnityObjectToWorldNormal(combinedNormal);
                o.vertex = UnityObjectToClipPos(newVertex); // Offset each vertex
                o.worldPos = mul(UNITY_MATRIX_M, float4(newVertex,1));
                triStream.Append(o);
                triStream.RestartStrip(); // Close the triangle strip
                
            }
                
            
            // frag func - last function in the flow (after the GPU do the rasterizer)
            float4 frag (g2f i) : SV_Target
            {
                float2 uv = i.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                fixed4 texColor = tex2D(_MainTex, uv);
                return texColor;
            }


            
            ENDCG
        }
    }
}