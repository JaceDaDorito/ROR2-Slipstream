using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

//This code is from SS2 btw

namespace Slipstream.PostProcess
{
    [PostProcess(typeof(SlipSobelOutlineRenderer), PostProcessEvent.BeforeTransparent, "Slip/SobelOutline", true)]
    [Serializable]
    public sealed class SlipSobelOutline : PostProcessEffectSettings
    {
		[Tooltip("The intensity of the outline.")]
		[Range(0f, 5f)]
		public FloatParameter outlineIntensity = new FloatParameter
		{
			value = 0.5f
		};

		[Range(0f, 10f)]
		[Tooltip("The falloff of the outline.")]
		public FloatParameter outlineScale = new FloatParameter
		{
			value = 1f
		};
	}
}
