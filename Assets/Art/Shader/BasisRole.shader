// Upgrade NOTE: upgraded instancing buffer 'CustomBasisRole' to new syntax.
// Upgrade NOTE: upgraded instancing buffer 'CustomBasisRoleOutline' to new syntax.

// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/BasisRole"
{
	Properties
	{
		_ASEOutlineColor( "Outline Color", Color ) = (0,0,0,0)
		_ASEOutlineWidth( "Outline Width", Float ) = 0.03
		_BaseTex("BaseTex", 2D) = "white" {}
		_NormalTex("NormalTex", 2D) = "bump" {}
		_Metallic("Metallic", Float) = 0
		_Smoothness("Smoothness", Float) = 0
		_Color0("Color 0", Color) = (1,1,1,1)
		_AOInt("AOInt", Float) = 0
		_EmissionColor("EmissionColor", Color) = (0,0,0,0)
		_NormalStr("NormalStr", Float) = 1.35
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ }
		Cull Front
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline nofog  keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nometa noforwardadd vertex:outlineVertexDataFunc 
		
		
		
		
		struct Input {
			half filler;
		};
		UNITY_INSTANCING_BUFFER_START(CustomBasisRoleOutline)
		UNITY_DEFINE_INSTANCED_PROP( float4, _ASEOutlineColor )
#define _ASEOutlineColor_arr CustomBasisRoleOutline
		UNITY_DEFINE_INSTANCED_PROP( float, _ASEOutlineWidth )
#define _ASEOutlineWidth_arr CustomBasisRoleOutline
		UNITY_INSTANCING_BUFFER_END(CustomBasisRoleOutline)
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz += ( v.normal * UNITY_ACCESS_INSTANCED_PROP(_ASEOutlineWidth_arr, _ASEOutlineWidth) );
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			o.Emission = UNITY_ACCESS_INSTANCED_PROP(_ASEOutlineColor_arr, _ASEOutlineColor).rgb;
			o.Alpha = 1;
		}
		ENDCG
		

		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma surface surf Standard keepalpha noshadow exclude_path:deferred novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _NormalTex;
		uniform float4 _NormalTex_ST;
		uniform float4 _Color0;
		uniform sampler2D _BaseTex;
		uniform float4 _BaseTex_ST;
		uniform float4 _EmissionColor;
		uniform float _Metallic;
		uniform float _Smoothness;
		uniform float _AOInt;

		UNITY_INSTANCING_BUFFER_START(CustomBasisRole)
			UNITY_DEFINE_INSTANCED_PROP(float, _NormalStr)
#define _NormalStr_arr CustomBasisRole
		UNITY_INSTANCING_BUFFER_END(CustomBasisRole)

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_NormalTex = i.uv_texcoord * _NormalTex_ST.xy + _NormalTex_ST.zw;
			float _NormalStr_Instance = UNITY_ACCESS_INSTANCED_PROP(_NormalStr_arr, _NormalStr);
			o.Normal = UnpackScaleNormal( tex2Dlod( _NormalTex, float4( uv_NormalTex, 0, 1.0) ), _NormalStr_Instance );
			float2 uv_BaseTex = i.uv_texcoord * _BaseTex_ST.xy + _BaseTex_ST.zw;
			o.Albedo = ( _Color0 * tex2D( _BaseTex, uv_BaseTex ) ).rgb;
			o.Emission = _EmissionColor.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Occlusion = _AOInt;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.TextureCoordinatesNode;6;-692,-70.5;Inherit;False;0;1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;19,-251.5;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;10;-442,-389.5;Inherit;False;Property;_Color0;Color 0;4;0;Create;True;0;0;0;False;0;False;1,1,1,1;1,0.7041115,0.259434,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-420,-86.5;Inherit;True;Property;_BaseTex;BaseTex;0;0;Create;True;0;0;0;False;0;False;-1;None;4784b1ccfcecf2246899768a99c6645c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;603,99;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Custom/BasisRole;False;False;False;False;False;True;True;True;True;True;True;True;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;False;0;False;Opaque;;Geometry;ForwardOnly;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;True;0.03;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.RangedFloatNode;3;46,426.5;Inherit;False;Property;_Metallic;Metallic;2;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;40,508.5;Inherit;False;Property;_Smoothness;Smoothness;3;0;Create;True;0;0;0;False;0;False;0;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;105.1873,595.1741;Inherit;False;Property;_AOInt;AOInt;5;0;Create;True;0;0;0;False;0;False;0;1.52;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;13;-53.00513,384.2654;Inherit;False;Property;_EmissionColor;EmissionColor;6;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;5;-841,69.5;Inherit;False;0;2;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-473,149.5;Inherit;True;Property;_NormalTex;NormalTex;1;0;Create;True;0;0;0;False;0;False;-1;None;66216e7ca9ce66e4599c007130a443ec;True;0;True;bump;Auto;True;Object;-1;MipLevel;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;14;-765.511,231.5652;Inherit;False;InstancedProperty;_NormalStr;NormalStr;7;0;Create;True;0;0;0;False;0;False;1.35;1.35;0;0;0;1;FLOAT;0
WireConnection;7;0;10;0
WireConnection;7;1;1;0
WireConnection;1;1;6;0
WireConnection;0;0;7;0
WireConnection;0;1;2;0
WireConnection;0;2;13;0
WireConnection;0;3;3;0
WireConnection;0;4;4;0
WireConnection;0;5;11;0
WireConnection;2;1;5;0
WireConnection;2;5;14;0
ASEEND*/
//CHKSM=9F8D1110F851D3BF1D646CC65D3D6F94C8826D67