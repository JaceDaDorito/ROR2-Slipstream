using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Slipstream.PostProcess
{
    public sealed class SlipSobelRainRenderer : PostProcessEffectRenderer<SlipSobelRain>
    {
		public override void Render(PostProcessRenderContext context)
		{
			PropertySheet propertySheet = context.propertySheets.Get(Shader.Find("Hidden/PostProcess/SobelRain"));
			propertySheet.properties.SetFloat("_RainIntensity", base.settings.rainIntensity);
			propertySheet.properties.SetFloat("_OutlineScale", base.settings.outlineScale);
			propertySheet.properties.SetFloat("_RainDensity", base.settings.rainDensity);
			propertySheet.properties.SetTexture("_RainTexture", base.settings.rainTexture);
			propertySheet.properties.SetColor("_RainColor", base.settings.rainColor);
			context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0, false, null);
		}
	}
}
