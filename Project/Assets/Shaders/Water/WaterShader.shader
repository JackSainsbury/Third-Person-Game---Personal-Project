Shader "Custom/Water"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		_DepthColor("Water Color", Color) = (1, 1, 1, 1)
		_DeepWaterColor("Depth Color", Color) = (1, 1, 1, 1)

		_DeepWaterAlpha("Deep Water Alpha", float) = 1.0

		_DepthFactor("Shore Depth Factor", float) = 1.0
		_CoreDepthFactor("Core Depth Factor", float) = 1.0

		_ShoreAnimSpeed("Shore animation Speed", float) = 1.0

		_WaveSpeed("Wave Speed", float) = 1.0
		_WaveAmp("Wave Amp", float) = 0.2
		_ShoreRampTex("Shore Ramp", 2D) = "white" {}
		_DepthRampTex("Depth Ramp", 2D) = "white" {}

	_NoiseTex("Noise Texture", 2D) = "white" {}

	_MainTex("Main Texture", 2D) = "white" {}
	_DistortStrength("Distort Strength", float) = 1.0
		_ExtraHeight("Extra Height", float) = 0.0
	}

		SubShader
	{
		Tags
	{
		"Queue" = "Transparent"
	}

		// Grab the screen behind the object into _BackgroundTexture
		GrabPass
	{
		"_BackgroundTexture"
	}

		// Background distortion
		Pass
	{

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

		// Properties
		sampler2D _BackgroundTexture;
	sampler2D _NoiseTex;
	float     _DistortStrength;
	float  _WaveSpeed;
	float  _WaveAmp;

	struct vertexInput
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
		float3 texCoord : TEXCOORD0;
	};

	struct vertexOutput
	{
		float4 pos : SV_POSITION;
		float4 grabPos : TEXCOORD0;
	};

	vertexOutput vert(vertexInput input)
	{
		vertexOutput output;

		// convert input to world space
		output.pos = UnityObjectToClipPos(input.vertex);
		float4 normal4 = float4(input.normal, 0.0);
		float3 normal = normalize(mul(normal4, unity_WorldToObject).xyz);

		// use ComputeGrabScreenPos function from UnityCG.cginc
		// to get the correct texture coordinate
		output.grabPos = ComputeGrabScreenPos(output.pos);

		// distort based on bump map
		float noiseSample = tex2Dlod(_NoiseTex, float4(input.texCoord.xy, 0, 0));
		output.grabPos.y += sin(_Time*_WaveSpeed*noiseSample)*_WaveAmp * _DistortStrength;
		output.grabPos.x += cos(_Time*_WaveSpeed*noiseSample)*_WaveAmp * _DistortStrength;

		return output;
	}

	float4 frag(vertexOutput input) : COLOR
	{
		return tex2Dproj(_BackgroundTexture, input.grabPos);
	}
		ENDCG
	}

		Pass
	{
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
#include "UnityCG.cginc"

#pragma vertex vert
#pragma fragment frag

		// Properties
		float4 _DepthColor;
		float4 _Color;
		float4 _DeepWaterColor;
		float _ShoreAnimSpeed;
	float  _DepthFactor;
	float  _CoreDepthFactor;
	float  _DeepWaterAlpha;
	float  _WaveSpeed;
	float  _WaveAmp;
	float _ExtraHeight;
	sampler2D _CameraDepthTexture;
	sampler2D _ShoreRampTex;
	sampler2D _DepthRampTex;
	sampler2D _NoiseTex;
	sampler2D _MainTex;

	float4 _MainTex_ST;

	struct vertexInput
	{
		float4 vertex : POSITION;
		float4 texCoord : TEXCOORD1;
	};

	struct vertexOutput
	{
		float4 pos : SV_POSITION;
		float2 texCoord : TEXCOORD0;
		float4 screenPos : TEXCOORD1;
		float2 texCoordRaw : TEXCOORD2;
	};

	vertexOutput vert(vertexInput input)
	{
		vertexOutput output;

		// convert to world space
		output.pos = UnityObjectToClipPos(input.vertex);

		// apply wave animation
		float noiseSample = tex2Dlod(_NoiseTex, float4(input.texCoord.xy, 0, 0));
		output.pos.y += sin(_Time*_WaveSpeed*noiseSample)*_WaveAmp + _ExtraHeight;
		output.pos.x += cos(_Time*_WaveSpeed*noiseSample)*_WaveAmp;
		
		// compute depth
		output.screenPos = ComputeScreenPos(output.pos);

		// texture coordinates 
		output.texCoord = TRANSFORM_TEX(input.texCoord, _MainTex);
		output.texCoordRaw = input.texCoord;

		return output;
	}

	float4 frag(vertexOutput input) : COLOR
	{
		// apply depth texture
		float4 depthSample = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, input.screenPos);
		float depth = LinearEyeDepth(depthSample).r;

		// create foamline
		float foamLine = 1 - saturate(_DepthFactor * (depth - input.screenPos.w));
		float foamLineCore = 1 - saturate(_CoreDepthFactor * (depth - input.screenPos.w));
		float DeepWaterVals = saturate(.3 * (depth - input.screenPos.w));

		float4 foamRamp = float4(tex2D(_ShoreRampTex, float2(foamLine, 0.5)).rgb, 1.0);
		float4 deepRamp = float4(tex2D(_DepthRampTex, float2(1 - DeepWaterVals, 0.5)).rgb, 1.0);

		// Get the alpha for invese of foam area
		float inverseFoam = foamLine > 0 ? 1 : 0;

		// sample main texture
		float offset = _Time * _ShoreAnimSpeed;
		float2 anim = float2(offset, 0);
		float4 albedo = tex2D(_MainTex, input.texCoord.xy + anim);

		float4 shore = (float4(foamRamp.rgb, inverseFoam) * _Color) + (albedo * foamLineCore * _DepthColor);
		float4 depths = float4(_DeepWaterColor.rgb, (1 - shore.a) * _DeepWaterColor.a);
		float4 cull = float4(deepRamp.rgb, (1 - (abs(input.texCoordRaw.y - .5) * 3)) * _DeepWaterAlpha);

		//depths = float4(depths.rgb * (deepRamp.rgb * _DeepWaterAlpha), saturate(depths.a + _DeepWaterAlpha));

		float4 col = cull + (float4(shore.rgb * shore.a, shore.a) + float4(depths.rgb * depths.a, depths.a));

		return saturate(col);
	}

		ENDCG
	}
	}
}