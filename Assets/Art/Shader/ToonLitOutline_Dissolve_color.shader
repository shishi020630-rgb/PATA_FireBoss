Shader "Custom/ToonOutline_Dissolve" {
	Properties{
		[Header(Toon Dissolve)]
		[Space(5)]
		[Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull mode", Float) = 2
		[Header(Textures)]
		[Space(5)]
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Ramp("Toon Ramp", 2D) = "white" {}
		_NoiseTex("Noise", 2D) = "white" {}
		[Header(Damage)]
		[Space(5)]
		[Toggle] _DamageOnOff("Damage On/Off", Float) = 0
		_RimColor("Damage Color", Color) = (1,1,1,1)
		[Header(Properties)]
		[Space(5)]
		_DissolveColor("Dissolved Edge Color", Color) = (1,0,0,1)
		_DissolvePower("Dissolve Edge Smooth", Range(0, 1)) = 0.15
		_Dissolved("Dissolved", Range(0,1)) = 0.0
		[Header(Outline)]
		[Space(5)]
		[MaterialToggle]_USEOUTLINE("Outline ON", Int) = 0
		_OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
		_OutlineWidth("Outline Width", Range(0, 0.1)) = 0


		  [Header(Shadow)]
        // _GroundHeight("_GroundHeight", Range(-100, 100)) = 0
        _GroundHeight("_GroundHeight", Float) = 0
        _ShadowColor("_ShadowColor", Color) = (0,0,0,1)
	_ShadowFalloff("_ShadowFalloff", Range(0,1)) = 0.05

	}
		SubShader{
			Tags { "RenderType" = "MKGlow" "Queue" = "Geometry"}
			Cull[_Cull]
			LOD 200

			Stencil 
			{
				Ref 1
				Comp Always
				Pass Replace
			}

			CGPROGRAM
			#pragma surface surf Ramp fullforwardshadows noforwardadd addshadow 
			//#pragma shader_feature DAMAGE			
			#pragma target 3.0

			sampler2D _Ramp;

			half4 LightingRamp(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
				half NdotL = dot(s.Normal, lightDir);
				half diff = NdotL * 0.5 + 0.5;
				half3 ramp = tex2D(_Ramp, float2(diff, diff)).rgb;
				half4 c;

				c.rgb = s.Albedo * _LightColor0.rgb * ramp * atten;
				c.a = s.Alpha;
				return c;
			}

			struct Input {
				float2 uv_MainTex;
				float2 uv_NoiseTex;
				float3 viewDir;
			};

			sampler2D _MainTex;
			sampler2D _NoiseTex;

			fixed4 _Color;
			fixed4 _RimColor;
			fixed4 _DissolveColor;
			half _Dissolved, _DamageOnOff;
			half _DissolvePower;


			void surf(Input IN, inout SurfaceOutput o) {

				float rim = 1 - saturate(dot(IN.viewDir, o.Normal));
				half noise = 1 - tex2D(_NoiseTex, IN.uv_NoiseTex).r;
				half dissolve = noise - _Dissolved * 1.01;

				clip(dissolve);
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

				half dissolvedBorder = _DissolvePower * saturate(_Dissolved * 10);

				if (dissolve < dissolvedBorder) {
					half cp = saturate(dissolve / dissolvedBorder);
					half bp = 1 - cp;
					c = (c * cp) + (_DissolveColor * bp);
				}
				o.Emission = _DamageOnOff *  _RimColor.rgb * pow(rim, 1);
				o.Albedo = c.rgb * _Color;
				o.Alpha = c.a;
			}
			ENDCG
			   // Planar Shadows平面阴影
        Pass
        {
            Name "PlanarShadow"

            //用使用模板测试以保证alpha显示正确
            Stencil
            {
                Ref 0
                Comp equal
                Pass incrWrap
                Fail keep
                ZFail keep
            }

            Cull Off

            //透明混合模式
            Blend SrcAlpha OneMinusSrcAlpha

            //关闭深度写入
            ZWrite off

            //深度稍微偏移防止阴影与地面穿插
            Offset -1 , 0

            CGPROGRAM
            #pragma shader_feature _CLIPPING
            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _ALPHAPREMULTIPLY_ON

            #include "UnityCG.cginc"

            #pragma vertex vert
            #pragma fragment frag
            
            float _GroundHeight;
            float4 _ShadowColor;
            float _ShadowFalloff;
            half4 _Tint;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Clipping;
            half _Cutoff;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            float3 ShadowProjectPos(float4 vertPos)
            {
                float3 shadowPos;

                //得到顶点的世界空间坐标
                float3 worldPos = mul(unity_ObjectToWorld , vertPos).xyz;

                //灯光方向
                // float3 lightDir = normalize(_LightDir.xyz);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

                //阴影的世界空间坐标（低于地面的部分不做改变）
                shadowPos.y = min(worldPos .y , _GroundHeight);
                shadowPos.xz = worldPos .xz - lightDir.xz * max(0 , worldPos .y - _GroundHeight) / lightDir.y; 

                return shadowPos;
            }

            float GetAlpha (v2f i) {
                float alpha = _Tint.a * tex2D(_MainTex, i.uv.xy).a;
                return alpha;
            }

            v2f vert (appdata v)
            {
                v2f o;

                //得到阴影的世界空间坐标
                float3 shadowPos = ShadowProjectPos(v.vertex);

                //转换到裁切空间
                o.vertex = UnityWorldToClipPos(shadowPos);

                //得到中心点世界坐标
                float3 center = float3(unity_ObjectToWorld[0].w , _GroundHeight , unity_ObjectToWorld[2].w);
                //计算阴影衰减
                float falloff = 1-saturate(distance(shadowPos , center) * _ShadowFalloff);

                //阴影颜色
                o.color = _ShadowColor;
                o.color.a *= falloff;
                
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                if (_Clipping)
                {
                    float alpha = GetAlpha(i);
                    i.color.a *= step(_Cutoff, alpha);
                }
                return i.color;
            }
            ENDCG
        }
			Pass{

			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Front
			//ZWrite Off

			Stencil 
			{
				Ref 1
				Comp Greater
			}

			CGPROGRAM
			#pragma shader_feature _USEOUTLINE_OFF _USEOUTLINE_ON
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
				uniform half4 _OutlineColor;
				uniform float _OutlineWidth;

			struct vertexInput {
			 	float4 pos: POSITION;
			 	float4 texcoord: TEXCOORD0;
			};

			struct vertexOutput {
				float4 pos: SV_POSITION;
			 	float4 texcoord: TEXCOORD0;
			};

			float4 Outline(float4 vertPos, float width)
			{
				float4x4 scaleMat;
				scaleMat[0][0] = 1.0 + width;
				scaleMat[0][1] = 0.0;
				scaleMat[0][2] = 0.0;
				scaleMat[0][3] = 0.0;
				scaleMat[1][0] = 0.0;
				scaleMat[1][1] = 1.0 + width;
				scaleMat[1][2] = 0.0;
				scaleMat[1][3] = 0.0;
				scaleMat[2][0] = 0.0;
				scaleMat[2][1] = 0.0;
				scaleMat[2][2] = 1.0 + width;
				scaleMat[2][3] = 0.0;
				scaleMat[3][0] = 0.0;
				scaleMat[3][1] = 0.0;
				scaleMat[3][2] = 0.0;
				scaleMat[3][3] = 1.0;	

				return mul(scaleMat, vertPos);
			}

			vertexOutput vert(vertexInput v) 
			{
					vertexOutput o;
			#if _USEOUTLINE_ON
					o.pos = UnityObjectToClipPos(Outline(v.pos,_OutlineWidth));
			#elif _USEOUTLINE_OFF
					o.pos = float4(0, 0, 0, 0);
			#endif
					return o;
			}

			half4 frag(vertexOutput i): COLOR 
			{
			 	return _OutlineColor;
			}
			ENDCG
			}
		}
	Fallback "Diffuse"
}