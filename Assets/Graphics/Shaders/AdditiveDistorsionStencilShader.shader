﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Additive Distortion Stencil" {
    Properties{
        _MainTex("Main Texture", 2D) = "white" {}
        _NoiseTex("Noise Texture", 2D) = "white" {}
        _Color ("Portal Color",Color) = (1,1,1,1)
        _IntensityAndScrolling("Intensity (XY), Scrolling (ZW)", Vector) = (0.1,0.1,0.1,0.1)

		_StencilReferenceID("Stencil ID Reference", Float) = 1
		[Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp("Stencil Comparison", Float) = 3
		[Enum(UnityEngine.Rendering.StencilOp)] _StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255
    }
    SubShader{
        Tags {
            "IgnoreProjector" = "True"
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }
        Pass {

        	Stencil
			{
				Ref[_StencilReferenceID]
				Comp[_StencilComp]	// always
				Pass[_StencilOp]	// replace
				ReadMask[_StencilReadMask]
				WriteMask[_StencilWriteMask]
			}

            Blend One One
 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
 
            #include "UnityCG.cginc"
 
            uniform sampler2D _MainTex;
            uniform sampler2D _NoiseTex;
            uniform float4 _MainTex_ST;
            uniform float4 _NoiseTex_ST;
            uniform float4 _IntensityAndScrolling;
            fixed4 _Color;
 
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
 
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
 
            VertexOutput vert(VertexInput v) {
                VertexOutput o;
                o.uv0 = TRANSFORM_TEX(v.texcoord0, _MainTex);
                o.uv1 = TRANSFORM_TEX(v.texcoord1, _NoiseTex);
                o.uv1 += _Time.yy * _IntensityAndScrolling.zw;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex);
 
                return o;
            }
 
            float4 frag(VertexOutput i) : COLOR {
                float4 noiseTex = tex2D(_NoiseTex, i.uv1);
                float2 offset = (noiseTex.rg * 2 - 1) * _IntensityAndScrolling.rg;
                float2 uvNoise = i.uv0 + offset;
                float4 mainTex = tex2D(_MainTex, uvNoise);
                float3 emissive = (mainTex.rgb * i.vertexColor.rgb) * (mainTex.a * i.vertexColor.a);
 
                return fixed4(emissive * _Color.rgb, 1);
            }
            ENDCG
        }
    }
    FallBack "Mobile/Particles/Additive"
}