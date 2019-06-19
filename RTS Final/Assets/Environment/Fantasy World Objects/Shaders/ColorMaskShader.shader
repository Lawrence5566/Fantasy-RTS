Shader "Toon/ColorMaskShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Mask("ColorMask (Red = Prim Green = Sec Blue = Ter)", 2D) = "white" {} // mask texture
		_Color1("Primary Color", Color) = (0.5,0.5,0.5,1) // primary color, replaces the red masked area
		[Toggle] _Emis1("Primary: Emissive?", Float) = 0 // sets the color as emissive if toggled
		_Color2("Secondary Color", Color) = (0.5,0.5,0.5,1) // secondary color, replaces green masked area
		[Toggle] _Emis2("Secondary: Emissive?", Float) = 0// sets the color as emissive if toggled
		_Color3("Tertiary Color", Color) = (0.5,0.5,0.5,1)// tertiary color, replaces blue masked area
		[Toggle] _Emis3("Tertiary: Emissive?", Float) = 0// sets the color as emissive if toggled
		_Color4("Quaternary Color", Color) = (0.5,0.5,0.5,1)// tertiary color, replaces blue masked area
		[Toggle] _Emis4("Quaternary: Emissive?", Float) = 0// sets the color as emissive if toggled
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows

		sampler2D _MainTex;
		sampler2D _Mask;
		float4 _Color1, _Color2, _Color3, _Color4;// custom colors
		float _Emis1, _Emis2, _Emis3, _Emis4; // emission toggles

		half _Glossiness;
		half _Metallic;

		struct Input {
			float2 uv_MainTex : TEXCOORD0;
		};

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float4 _Color = float4(0, 0, 0, 0);
			half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			half4 m = tex2D(_Mask, IN.uv_MainTex); // mask based on the uvs

			float3 c1 = _Color1 * m.r;
			float3 c2 = _Color2 * m.g;
			float3 c3 = _Color3 * m.b;
			float3 c4 = _Color4 * (1.0 - m.a); // invert the alpha value

			o.Albedo = c1 + c2 + c3 + c4; // all parts added together form the new look for the model
			o.Emission = c1 * _Emis1 + c2 * _Emis2 + c3 * _Emis3 + c4 * _Emis4; // emissive only shows up when the toggles for their colours are toggled
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG

	}

	FallBack "Diffuse"
}