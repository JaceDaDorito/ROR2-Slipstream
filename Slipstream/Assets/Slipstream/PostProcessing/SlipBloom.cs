﻿using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

//This code is from SS2 btw

namespace Slipstream.PostProcess
{
	[PostProcess(typeof(SlipBloomRenderer), PostProcessEvent.BeforeTransparent, "Slip/Bloom", true)]
	[Serializable]
	public sealed class SlipBloom : PostProcessEffectSettings
	{
		[Tooltip("Strength of the bloom filter. Values higher than 1 will make bloom contribute more energy to the final render.")]
		public FloatParameter intensity = new FloatParameter
		{
			value = 1f
		};

		[Tooltip("Filters out pixels under this level of brightness. Value is in gamma-space.")]
		public FloatParameter threshold = new FloatParameter
		{
			value = 1f
		};

		[Range(0f, 1f)]
		[Tooltip("Makes transitions between under/over-threshold gradual. 0 for a hard threshold, 1 for a soft threshold).")]
		public FloatParameter softKnee = new FloatParameter
		{
			value = 0f
		};

		[Tooltip("Clamps pixels to control the bloom amount. Value is in gamma-space.")]
		public FloatParameter clamp = new FloatParameter
		{
			value = 1f
		};

		[Range(1f, 10f)]
		[Tooltip("Changes the extent of veiling effects. For maximum quality, use integer values. Because this value changes the internal iteration count, You should not animating it as it may introduce issues with the perceived radius.")]
		public FloatParameter diffusion = new FloatParameter
		{
			value = 0f
		};

		[Range(-1f, 1f)]
		[Tooltip("Distorts the bloom to give an anamorphic look. Negative values distort vertically, positive values distort horizontally.")]
		public FloatParameter anamorphicRatio = new FloatParameter
		{
			value = 0f
		};

		[Tooltip("Global tint of the bloom filter.")]
		public ColorParameter color = new ColorParameter
		{
			value = Color.white
		};

		[Tooltip("Boost performance by lowering the effect quality. This settings is meant to be used on mobile and other low-end platforms but can also provide a nice performance boost on desktops and consoles.")]
		public BoolParameter fastMode = new BoolParameter
		{
			value = false
        };

		[Tooltip("The lens dirt texture used to add smudges or dust to the bloom effect.")]
		[DisplayName("Texture")]
		public TextureParameter dirtTexture = new TextureParameter
		{
			value = null
		};

		[UnityEngine.Rendering.PostProcessing.Min(0F)]
		[Tooltip("The intensity of the lens dirtiness.")]
		[DisplayName("Intensity")]
		public FloatParameter dirtIntensity = new FloatParameter
		{
			value = 1f
		};

	}

}
