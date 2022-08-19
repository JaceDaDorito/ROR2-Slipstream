using System;
using UnityEngine;
using Moonstorm.Components;

namespace Slipstream.Components
{
    public class HGTriplanarController : MaterialControllerComponents.MaterialController
    {
        public bool _UseVertexColorsInstead;
        public bool _MixColorsOn;
        public bool _MaskOn;
        public bool _VerticalBiasOn;
        public bool _DoubleSampleOn;

        public Color _MainColor;
        public Texture _NormalTex;

        public float _NormalStrength;

    }
}