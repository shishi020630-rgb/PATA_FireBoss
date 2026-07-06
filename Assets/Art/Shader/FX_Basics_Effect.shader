// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/BasicsEffect"
{
	Properties
	{
		[Enum(Off,0,On,1)]_Depth("Depth", Float) = 0
		_DepthFade("DepthFade", Float) = 0
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode("CullMode", Float) = 0
		[Enum(Add,1,Blend,10)]_BlendMode("BlendMode", Float) = 0
		[Enum(Appha,0,R_Channel,1)]_MianChannel("MianChannel", Float) = 0
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("MainTex", 2D) = "white" {}
		_MianTexUVRotator("MianTexUVRotator", Range( 0 , 1)) = 0
		[Enum(UVSpeed,0,UVOnce,1)]_UVOnce("UVOnce", Float) = 0
		_MianTexUVSpeed("MianTexUVSpeed", Vector) = (0,0,0,0)
		[Enum(R_Channel,0,Aphla,1)]_MaskChannel("MaskChannel", Float) = 0
		_MaskTex("MaskTex", 2D) = "white" {}
		_MaskTexUVRotator("MaskTexUVRotator", Range( 0 , 1)) = 0
		_MaskTexUVSpeed("MaskTexUVSpeed", Vector) = (0,0,0,0)
		_DissolveTex("DissolveTex", 2D) = "white" {}
		_EdgeColor("EdgeColor", Color) = (0,0,0,0)
		_SmoothEdge("SmoothEdge", Range( 0 , 1)) = 0
		_EdgeWidth("EdgeWidth", Range( 0 , 1)) = 0
		_DissolveTexUVRotator("DissolveTexUVRotator", Range( 0 , 1)) = 0
		_DissolveTexUVSpeed("DissolveTexUVSpeed", Vector) = (0,0,0,0)
		[Enum(Off,0,ON,1)]_NoiseSwitch("NoiseSwitch", Float) = 0
		_NosieInt("NosieInt", Float) = 0
		_NoiseTex("NoiseTex", 2D) = "bump" {}
		_NoiseTexUVSpeed("NoiseTexUVSpeed", Vector) = (0,0,0,0)

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
	LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend SrcAlpha [_BlendMode]
		AlphaToMask Off
		Cull [_CullMode]
		ColorMask RGBA
		ZWrite [_Depth]
		ZTest LEqual
		
		
		
		Pass
		{
			Name "Unlit"

			CGPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"
			#define ASE_NEEDS_FRAG_COLOR


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_color : COLOR;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform float _CullMode;
			uniform float _BlendMode;
			uniform float _Depth;
			uniform float4 _Color;
			uniform sampler2D _MainTex;
			uniform sampler2D _NoiseTex;
			uniform float2 _NoiseTexUVSpeed;
			uniform float4 _NoiseTex_ST;
			uniform float _NosieInt;
			uniform float _NoiseSwitch;
			uniform float2 _MianTexUVSpeed;
			uniform float4 _MainTex_ST;
			uniform float _MianTexUVRotator;
			uniform float _UVOnce;
			uniform float _SmoothEdge;
			uniform sampler2D _DissolveTex;
			uniform float2 _DissolveTexUVSpeed;
			uniform float4 _DissolveTex_ST;
			uniform float _DissolveTexUVRotator;
			uniform float _EdgeWidth;
			uniform float4 _EdgeColor;
			uniform float _MianChannel;
			uniform sampler2D _MaskTex;
			uniform float2 _MaskTexUVSpeed;
			uniform float4 _MaskTex_ST;
			uniform float _MaskTexUVRotator;
			uniform float _MaskChannel;
			UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
			uniform float4 _CameraDepthTexture_TexelSize;
			uniform float _DepthFade;

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float4 ase_clipPos = UnityObjectToClipPos(v.vertex);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord3 = screenPos;
				
				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				o.ase_texcoord2 = v.ase_texcoord1;
				o.ase_color = v.color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.zw = 0;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = vertexValue;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 WorldPosition = i.worldPos;
				#endif
				float2 temp_cast_0 = (0.0).xx;
				float2 uv_NoiseTex = i.ase_texcoord1.xy * _NoiseTex_ST.xy + _NoiseTex_ST.zw;
				float2 panner64 = ( 1.0 * _Time.y * _NoiseTexUVSpeed + uv_NoiseTex);
				float2 NosieInt69 = ( (UnpackNormal( tex2D( _NoiseTex, panner64 ) )).xy * _NosieInt );
				float2 lerpResult85 = lerp( temp_cast_0 , NosieInt69 , _NoiseSwitch);
				float2 uv_MainTex = i.ase_texcoord1.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float cos73 = cos( ( _MianTexUVRotator * UNITY_PI ) );
				float sin73 = sin( ( _MianTexUVRotator * UNITY_PI ) );
				float2 rotator73 = mul( uv_MainTex - float2( 0.5,0.5 ) , float2x2( cos73 , -sin73 , sin73 , cos73 )) + float2( 0.5,0.5 );
				float2 panner72 = ( 1.0 * _Time.y * _MianTexUVSpeed + rotator73);
				float4 texCoord80 = i.ase_texcoord2;
				texCoord80.xy = i.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult82 = (float2(texCoord80.x , texCoord80.y));
				float2 lerpResult88 = lerp( panner72 , ( rotator73 + appendResult82 ) , _UVOnce);
				float4 tex2DNode3 = tex2D( _MainTex, ( lerpResult85 + lerpResult88 ) );
				float clampResult17 = clamp( _SmoothEdge , 0.0 , 0.5 );
				float temp_output_23_0 = ( 1.0 - clampResult17 );
				float2 uv_DissolveTex = i.ase_texcoord1.xy * _DissolveTex_ST.xy + _DissolveTex_ST.zw;
				float cos43 = cos( ( _DissolveTexUVRotator * UNITY_PI ) );
				float sin43 = sin( ( _DissolveTexUVRotator * UNITY_PI ) );
				float2 rotator43 = mul( uv_DissolveTex - float2( 0.5,0.5 ) , float2x2( cos43 , -sin43 , sin43 , cos43 )) + float2( 0.5,0.5 );
				float2 panner41 = ( 1.0 * _Time.y * _DissolveTexUVSpeed + rotator43);
				float4 tex2DNode8 = tex2D( _DissolveTex, panner41 );
				float4 texCoord12 = i.ase_texcoord2;
				texCoord12.xy = i.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_10_0 = ( ( tex2DNode8.r * tex2DNode8.a ) + ( 1.0 - ( texCoord12.z * 2.0 ) ) );
				float smoothstepResult22 = smoothstep( clampResult17 , temp_output_23_0 , ( temp_output_10_0 + _EdgeWidth ));
				float smoothstepResult24 = smoothstep( clampResult17 , temp_output_23_0 , temp_output_10_0);
				float3 EdgeColor36 = ( ( smoothstepResult22 - smoothstepResult24 ) * (_EdgeColor).rgb );
				float lerpResult99 = lerp( tex2DNode3.a , tex2DNode3.r , _MianChannel);
				float2 uv_MaskTex = i.ase_texcoord1.xy * _MaskTex_ST.xy + _MaskTex_ST.zw;
				float cos50 = cos( ( _MaskTexUVRotator * UNITY_PI ) );
				float sin50 = sin( ( _MaskTexUVRotator * UNITY_PI ) );
				float2 rotator50 = mul( uv_MaskTex - float2( 0.5,0.5 ) , float2x2( cos50 , -sin50 , sin50 , cos50 )) + float2( 0.5,0.5 );
				float2 panner53 = ( 1.0 * _Time.y * _MaskTexUVSpeed + rotator50);
				float4 tex2DNode47 = tex2D( _MaskTex, panner53 );
				float lerpResult54 = lerp( tex2DNode47.r , tex2DNode47.a , _MaskChannel);
				float Mask57 = lerpResult54;
				float DissolveInt39 = smoothstepResult22;
				float4 appendResult38 = (float4(( ( (_Color).rgb * (tex2DNode3).rgb * (i.ase_color).rgb ) + EdgeColor36 ) , ( _Color.a * lerpResult99 * i.ase_color.a * Mask57 * DissolveInt39 )));
				float4 screenPos = i.ase_texcoord3;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth92 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
				float distanceDepth92 = abs( ( screenDepth92 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _DepthFade ) );
				
				
				finalColor = ( appendResult38 * saturate( distanceDepth92 ) );
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	Fallback Off
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.CommentaryNode;46;-2810.76,1239.238;Inherit;False;3810.167;1098.298;;28;15;8;9;11;18;19;16;17;10;21;20;23;24;28;22;25;32;26;36;39;44;45;43;42;41;40;13;12;Dissolve;0.6226415,0,0.4045247,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;90;-4147.244,-498.5515;Inherit;False;1803.148;415.9193;;8;65;66;64;63;68;67;69;84;Nosie;0.6603774,0.5550773,0.1962442,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;45;-2752,1472;Inherit;False;Property;_DissolveTexUVRotator;DissolveTexUVRotator;18;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PiNode;44;-2416,1472;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;40;-2560,1296;Inherit;False;0;8;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;65;-4097.244,-448.5515;Inherit;False;0;63;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;66;-4086.243,-273.5515;Inherit;False;Property;_NoiseTexUVSpeed;NoiseTexUVSpeed;23;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RotatorNode;43;-2208,1344;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;64;-3720.922,-420.3929;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;42;-2498,1581;Inherit;False;Property;_DissolveTexUVSpeed;DissolveTexUVSpeed;19;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;12;-2206.785,1681.866;Inherit;False;1;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;41;-1936,1424;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-1930.264,1947.123;Inherit;False;Constant;_Float0;Float 0;5;0;Create;True;0;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;63;-3455.941,-428.0932;Inherit;True;Property;_NoiseTex;NoiseTex;22;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;62;-2808.917,553.6762;Inherit;False;1980.166;579;;10;48;50;53;51;49;52;47;54;55;57;Mask;0.4528302,0,0,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-1627.538,1709.82;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;75;-1970.302,86.27057;Inherit;False;Property;_MianTexUVRotator;MianTexUVRotator;7;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;8;-1678.655,1433.961;Inherit;True;Property;_DissolveTex;DissolveTex;14;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;84;-3073.81,-419.5224;Inherit;False;True;True;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;68;-3032.853,-195.6323;Inherit;False;Property;_NosieInt;NosieInt;21;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-1213.537,1488.82;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-1578.538,2114.82;Inherit;False;Constant;_Float1;Float 1;6;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-1580.538,2206.82;Inherit;False;Constant;_Float2;Float 2;6;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-1619.538,1996.82;Inherit;False;Property;_SmoothEdge;SmoothEdge;16;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;49;-2758.917,780.6763;Inherit;False;Property;_MaskTexUVRotator;MaskTexUVRotator;12;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;70;-1602.663,-103.7143;Inherit;False;0;3;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PiNode;74;-1617.302,62.27057;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;11;-1259.537,1708.82;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;67;-2769.056,-348.3609;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;80;-1900.876,293.5529;Inherit;False;1;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;17;-1289.833,2087.863;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;10;-1041.916,1512.467;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-828.0046,1653.13;Inherit;False;Property;_EdgeWidth;EdgeWidth;17;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PiNode;48;-2422.919,779.6763;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;52;-2560,608;Inherit;False;0;47;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;82;-1325.876,321.5529;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RotatorNode;73;-1355.302,-92.72943;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;69;-2586.096,-333.3948;Inherit;False;NosieInt;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;71;-1368.843,93.30505;Inherit;False;Property;_MianTexUVSpeed;MianTexUVSpeed;9;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;20;-501.4865,1531.355;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;23;-782.5029,2224.536;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;50;-2214.921,651.6763;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;51;-2528,976;Inherit;False;Property;_MaskTexUVSpeed;MaskTexUVSpeed;13;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;78;-1108.505,307.5332;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;77;-833.6309,-271.8787;Inherit;False;69;NosieInt;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;86;-849.9701,-405.9428;Inherit;False;Constant;_Float0;Float 0;19;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;72;-1022.635,-82.03824;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;89;-772.904,391.8383;Inherit;False;Property;_UVOnce;UVOnce;8;1;[Enum];Create;True;0;2;UVSpeed;0;UVOnce;1;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;87;-764.1514,-119.3916;Inherit;False;Property;_NoiseSwitch;NoiseSwitch;20;1;[Enum];Create;True;0;2;Off;0;ON;1;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;24;-244.5564,2089.377;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;22;-280.1282,1642.34;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;53;-1944.585,735.0023;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;85;-572.4052,-289.2486;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;88;-517.2103,258.9787;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;28;9.775926,2113.437;Inherit;False;Property;_EdgeColor;EdgeColor;15;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;47;-1715.861,708.4525;Inherit;True;Property;_MaskTex;MaskTex;11;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;76;-241.0056,-58.94119;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;55;-1473.33,937.7535;Inherit;False;Property;_MaskChannel;MaskChannel;10;1;[Enum];Create;True;0;2;R_Channel;0;Aphla;1;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;32;251.2543,2012.563;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;25;63.22204,1740.113;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;475.5999,1697.543;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;54;-1263.33,732.7535;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;6;242.8843,178.2163;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;5;196.3928,-310.9836;Inherit;False;Property;_Color;Color;5;0;Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;36;757.4069,1690.396;Inherit;False;EdgeColor;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;57;-1070.756,722.428;Inherit;False;Mask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;33;541.7527,-247.4717;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;39;44.71899,1583.208;Inherit;False;DissolveInt;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;893.1238,-58.10212;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;60;732.4225,362.7142;Inherit;False;57;Mask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;61;733.4971,474.3025;Inherit;False;39;DissolveInt;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;1040.032,262.7941;Inherit;False;5;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;29;1346.389,-60.8913;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;1;-388.979,-488.6177;Inherit;False;Property;_CullMode;CullMode;2;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.CullMode;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-201.2144,-484.2059;Inherit;False;Property;_BlendMode;BlendMode;3;1;[Enum];Create;True;0;2;Add;1;Blend;10;0;True;0;False;0;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;38;1800.915,63.15441;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;2250.908,4.508018;Float;False;True;-1;2;ASEMaterialInspector;100;5;Custom/BasicsEffect;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;True;1;5;False;;0;True;_BlendMode;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;True;True;0;True;_CullMode;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;2;True;_Depth;True;0;False;;True;False;0;False;;0;False;;True;2;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;1;True;False;;False;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;97;2045.381,196.6407;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SaturateNode;98;1862.381,420.6407;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;92;1543.391,431.3316;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;37;1030.017,112.0502;Inherit;False;36;EdgeColor;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;91;-374.0977,-394.0947;Inherit;False;Property;_Depth;Depth;0;1;[Enum];Create;True;0;2;Off;0;On;1;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;93;1323.059,453.9742;Inherit;False;Property;_DepthFade;DepthFade;1;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;34;588.299,-83.69778;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;35;523.496,232.8368;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;99;623.6949,54.34056;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;3;201.2891,-62.97351;Inherit;True;Property;_MainTex;MainTex;6;0;Create;True;0;0;0;False;0;False;-1;None;88104208043120844b19d5a07850b963;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;100;468.7645,138.9472;Inherit;False;Property;_MianChannel;MianChannel;4;1;[Enum];Create;True;0;2;Appha;0;R_Channel;1;0;False;0;False;0;0;0;0;0;1;FLOAT;0
WireConnection;44;0;45;0
WireConnection;43;0;40;0
WireConnection;43;2;44;0
WireConnection;64;0;65;0
WireConnection;64;2;66;0
WireConnection;41;0;43;0
WireConnection;41;2;42;0
WireConnection;63;1;64;0
WireConnection;13;0;12;3
WireConnection;13;1;15;0
WireConnection;8;1;41;0
WireConnection;84;0;63;0
WireConnection;9;0;8;1
WireConnection;9;1;8;4
WireConnection;74;0;75;0
WireConnection;11;0;13;0
WireConnection;67;0;84;0
WireConnection;67;1;68;0
WireConnection;17;0;16;0
WireConnection;17;1;18;0
WireConnection;17;2;19;0
WireConnection;10;0;9;0
WireConnection;10;1;11;0
WireConnection;48;0;49;0
WireConnection;82;0;80;1
WireConnection;82;1;80;2
WireConnection;73;0;70;0
WireConnection;73;2;74;0
WireConnection;69;0;67;0
WireConnection;20;0;10;0
WireConnection;20;1;21;0
WireConnection;23;0;17;0
WireConnection;50;0;52;0
WireConnection;50;2;48;0
WireConnection;78;0;73;0
WireConnection;78;1;82;0
WireConnection;72;0;73;0
WireConnection;72;2;71;0
WireConnection;24;0;10;0
WireConnection;24;1;17;0
WireConnection;24;2;23;0
WireConnection;22;0;20;0
WireConnection;22;1;17;0
WireConnection;22;2;23;0
WireConnection;53;0;50;0
WireConnection;53;2;51;0
WireConnection;85;0;86;0
WireConnection;85;1;77;0
WireConnection;85;2;87;0
WireConnection;88;0;72;0
WireConnection;88;1;78;0
WireConnection;88;2;89;0
WireConnection;47;1;53;0
WireConnection;76;0;85;0
WireConnection;76;1;88;0
WireConnection;32;0;28;0
WireConnection;25;0;22;0
WireConnection;25;1;24;0
WireConnection;26;0;25;0
WireConnection;26;1;32;0
WireConnection;54;0;47;1
WireConnection;54;1;47;4
WireConnection;54;2;55;0
WireConnection;36;0;26;0
WireConnection;57;0;54;0
WireConnection;33;0;5;0
WireConnection;39;0;22;0
WireConnection;4;0;33;0
WireConnection;4;1;34;0
WireConnection;4;2;35;0
WireConnection;58;0;5;4
WireConnection;58;1;99;0
WireConnection;58;2;6;4
WireConnection;58;3;60;0
WireConnection;58;4;61;0
WireConnection;29;0;4;0
WireConnection;29;1;37;0
WireConnection;38;0;29;0
WireConnection;38;3;58;0
WireConnection;0;0;97;0
WireConnection;97;0;38;0
WireConnection;97;1;98;0
WireConnection;98;0;92;0
WireConnection;92;0;93;0
WireConnection;34;0;3;0
WireConnection;35;0;6;0
WireConnection;99;0;3;4
WireConnection;99;1;3;1
WireConnection;99;2;100;0
WireConnection;3;1;76;0
ASEEND*/
//CHKSM=BB54D8C23A6F324CDC496BD593CA20AF7E21A24E