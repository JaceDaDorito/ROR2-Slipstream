using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

//This code is from SS2 btw

namespace Slipstream.PostProcess
{
	public class SlipBloomRenderer : PostProcessEffectRenderer<SlipBloom>
	{
		public override void Render(PostProcessRenderContext context)
		{
			PropertySheet propertySheet = context.propertySheets.Get(Shader.Find("Hidden/PostProcessing/Bloom"));
			propertySheet.properties.SetFloat("_Intensity", settings.intensity);
			propertySheet.properties.SetFloat("_Threshold", settings.threshold);
			propertySheet.properties.SetFloat("_SoftKnee", settings.softKnee);
			propertySheet.properties.SetFloat("_Clamp", settings.clamp);
			propertySheet.properties.SetFloat("_Diffusion", settings.diffusion);
			propertySheet.properties.SetFloat("_AnamorphicRatio", settings.anamorphicRatio);
			propertySheet.properties.SetColor("_Color", settings.color);
			propertySheet.properties.SetInt("_FastMode", settings.fastMode ? 1: 0);
			propertySheet.properties.SetTexture("_DirtTexture", settings.dirtTexture);
			propertySheet.properties.SetFloat("_DirtIntensity", settings.dirtIntensity);
			context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0, false, null);
		}
	}
}

