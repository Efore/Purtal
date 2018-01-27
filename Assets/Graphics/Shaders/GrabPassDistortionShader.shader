// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Grab Pass Distortion"
{
	Properties
	{
		_DistortionTex ("Distortion Texture", 2D) = "white" {}
		_DistortionValue ("Distortion Value", Range (0.01, 2)) = 0.01

		_StencilReferenceID("Stencil ID Reference", Float) = 1
		[Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp("Stencil Comparison", Float) = 3
		[Enum(UnityEngine.Rendering.StencilOp)] _StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255
	}

	SubShader
	{
		Tags { "Queue"="Transparent" }
		GrabPass
        {
            "_BackgroundTexture"
        }
		Pass
		{
			Stencil
			{
				Ref[_StencilReferenceID]
				Comp[_StencilComp]	// always
				Pass[_StencilOp]	// replace
				ReadMask[_StencilReadMask]
				WriteMask[_StencilWriteMask]
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
            {
            	float2 uv : TEXCOORD0;
                float4 grabPos : TEXCOORD1;
                float4 pos : SV_POSITION;
            };

			sampler2D _DistortionTex;
			float _DistortionValue;
			float _Adjustment;

			sampler2D _BackgroundTexture;
			float4 _MainTex_ST;
			
		  	v2f vert(appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.pos);
                o.uv = v.uv;
                return o;
            }
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 texCol = tex2D(_DistortionTex , i.uv );
				float2 direction = float2(0.5,0.5) - i.uv; 
				direction = normalize(direction);
				i.grabPos.x = i.grabPos.x + direction.x * _DistortionValue * (1- texCol.r);
				i.grabPos.y = i.grabPos.y + direction.y * _DistortionValue * (1 -texCol.r);
				fixed4 col = tex2Dproj(_BackgroundTexture, UNITY_PROJ_COORD(i.grabPos));
				return col;
			}
			ENDCG
		}
	}
}
