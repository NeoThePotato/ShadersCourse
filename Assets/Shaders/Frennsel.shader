Shader "Unlit/Frennsel" // shader name  - shader1 under shader category - Unlit
{
	// properties to be expose in the material
	Properties
	{
		// Color Property: Used for specifying color values.
        _Color("Color", Color) = (1, 1, 1)
		// Float Property: Used for specifying single floating-point values.
		_Power("Power", Range(1, 4)) = 1 //with a specified range.
	}
	// specific shader (there might be more then on subshader) under one defined shader.
	SubShader
	{
		Tags { "RenderType"="Transparent" } // common tags: Opaque, Transparent, Overlay,
		LOD 100

		
		//blending parameters
		// SrcFactor is this color DesFactor is the color that is been render before at this vert position
		// syntax: Blend SrcFactor DstFactor
		// FinalColor = (SourceColor × SrcFactor) + (DestinationColor × DstFactor)
		
		Blend SrcAlpha OneMinusSrcAlpha //Specifies the blend mode 
		ZWrite Off //writes to the depth buffer 
		ZTest LEqual //depth comparison function 
		
		Cull Back //face culling mode 

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
			#pragma fragment frag // define the name of the fragment func
			
			#include "UnityCG.cginc"

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

			 // structure to pass from vert to frag after also passing at the rasterizer
			struct v2f
			{
				float4 vertex : SV_POSITION; // use container name or "POSITION" - this will use for clip space pos
				float3 worldNormal : NORMAL;
				float3 viewDirection: TEXCOORD0;
			};

			
			// declare shaders variables
			// syntax: <PropertyType> <VariableName>;
			float3 _Color;
			float _Power;

			// vertex func - first function in the flow
			v2f vert (appdata v)
			{
				v2f o;
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
				
				o.vertex = UnityObjectToClipPos(v.vertex); // from object directly to clip (camera) pos
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.viewDirection = normalize(_WorldSpaceCameraPos - worldPos);
				return o;
			}

			// frag func - last function in the flow (after the GPU do the rasterizer)
			float4 frag (v2f i) : SV_Target
			{
				float strength = dot(i.viewDirection, i.worldNormal);
				strength = saturate(strength);
				strength = 1 - strength;
				strength = pow(strength, _Power);
				return float4(_Color, strength);
			}
			ENDCG
		}
	}
}