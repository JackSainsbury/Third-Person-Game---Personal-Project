// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/SkinnedSway" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}

		_Frequency ("Frequency", float) = 0.0
		_SpacialFrequency ("Spacial Frequency", float) = 0.0
		_WaveLength ("Wave Length", float) = 0.0
		_WaveSize ("Wave Size", float) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" "DisableBatching" = "True"}
		LOD 200
		
		CGPROGRAM 

		#pragma surface surf CelShadingForward fullforwardshadows vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		half4 LightingCelShadingForward(SurfaceOutput s, half3 lightDir, half atten) {
			half NdotL = dot(s.Normal, lightDir);
			if (NdotL <= 0.0) NdotL = 0;
			else NdotL = 1;
			half4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * atten * 2);
			c.a = s.Alpha;
			return c;
		}

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		fixed4 _Color;

		float _Frequency;
		float _SpacialFrequency;
		float _WaveLength;
		float _WaveSize;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void vert (inout appdata_full v) {
			float3 worldPos = mul (unity_ObjectToWorld, v.vertex).xyz;

			//waving mesh
			v.vertex.x += sin((v.vertex.y + (_Time + (worldPos.x * _SpacialFrequency)) * _Frequency) / _WaveLength) * _WaveSize;
			v.vertex.z += cos((v.vertex.y + (_Time + (worldPos.z * _SpacialFrequency)) * _Frequency) / _WaveLength) * _WaveSize;
		}

		void surf(Input IN, inout SurfaceOutput o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
