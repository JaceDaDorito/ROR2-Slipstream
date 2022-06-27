using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

//This code is from SS2 btw

namespace Slipstream.PostProcess
{
	public sealed class SlipSobelOutlineRenderer : PostProcessEffectRenderer<SlipSobelOutline>
	{
		public override void Render(PostProcessRenderContext context)
		{
			PropertySheet propertySheet = context.propertySheets.Get(Shader.Find("Hidden/PostProcess/SobelOutline"));
			propertySheet.properties.SetFloat("_OutlineIntensity", base.settings.outlineIntensity);
			propertySheet.properties.SetFloat("_OutlineScale", base.settings.outlineScale);
			context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0, false, null);
		}
	}
}
