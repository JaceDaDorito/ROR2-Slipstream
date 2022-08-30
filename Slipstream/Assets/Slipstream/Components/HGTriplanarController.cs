using System;
using UnityEngine;
using Moonstorm.Components;

namespace Slipstream.Components
{
    public class HGTriplanarController : MaterialControllerComponents.MaterialController
    {
        public bool _ColorsOn;
        public bool _MixColorsOn;
        public bool _MaskOn;
        public bool _VerticalBiasOn;
        public bool _DoubleSampleOn;

        public Color _Color;
        public Texture _NormalTex;

        [Range(0f, 1f)]
        public float _NormalStrength;

        public enum _RampInfoEnum
        {
            TwoTone = 0,
            SmoothedTwoTone = 1,
            Unlitish = 2,
            Subsurface = 3,
            Grass = 4
        }
        public _RampInfoEnum _RampInfo;

        public enum _DecalLayerEnum
        {
            Default = 0,
            Environment = 1,
            Character = 2,
            Misc = 3
        }
        public _DecalLayerEnum _DecalLayer;

        public enum _CullEnum
        {
            Off = 0,
            Front = 1,
            Back = 2
        }
        public _CullEnum _Cull;

        [Range(0f, 1f)]
        public float _TextureFactor;

        [Range(0f, 1f)]
        public float _Depth;

        public Texture _SplatmapTex;

        public Texture _RedChannelTopTex;
        public Texture _RedChannelSideTex;
        [Range(0f, 1f)]
        public float _RedChannelSmoothness;
        [Range(0f, 1f)]
        public float _RedChannelSpecularStrength;
        [Range(0.1f, 20f)]
        public float _RedChannelSpecularExponent;
        [Range(-2f, 5f)]
        public float _RedChannelBias;

        public Texture _GreenChannelTex;
        [Range(0f, 1f)]
        public float _GreenChannelSmoothness;
        [Range(0f, 1f)]
        public float _GreenChannelSpecularStrength;
        [Range(0.1f, 20f)]
        public float _GreenChannelSpecularExponent;
        [Range(-2f, 5f)]
        public float _GreenChannelBias;

        public Texture _BlueChannelTex;
        [Range(0f, 1f)]
        public float _BlueChannelSmoothness;
        [Range(0f, 1f)]
        public float _BlueChannelSpecularStrength;
        [Range(0.1f, 20f)]
        public float _BlueChannelSpecularExponent;
        [Range(-2f, 5f)]
        public float _BlueChannelBias;

        public bool _SnowOn;

        public void Start()
        {
            GrabMaterialValues();
        }

        public void GrabMaterialValues()
        {
            if (material)
            {
                _ColorsOn = material.IsKeywordEnabled("USE_VERTEX_COLORS");
                _MixColorsOn = material.IsKeywordEnabled("MIX_VERTEX_COLORS");
                _MaskOn = material.IsKeywordEnabled("USE_ALPHA_AS_MASK");
                _VerticalBiasOn = material.IsKeywordEnabled("USE_VERTICAL_BIAS");
                _DoubleSampleOn = material.IsKeywordEnabled("DOUBLESAMPLE");
                _Color = material.GetColor("_Color");//
                _NormalTex = material.GetTexture("_NormalTex");//
                _NormalStrength = material.GetFloat("_NormalStrength");//
                _RampInfo = (_RampInfoEnum)(int)material.GetFloat("_RampInfo");//
                _DecalLayer = (_DecalLayerEnum)(int)material.GetFloat("_DecalLayer");//
                _Cull = (_CullEnum)(int)material.GetFloat("_Cull");//
                _TextureFactor = material.GetFloat("_TextureFactor");//
                _Depth = material.GetFloat("_Depth");
                _SplatmapTex = material.GetTexture("_SplatmapTex");
                _RedChannelTopTex = material.GetTexture("_RedChannelTopTex");
                _RedChannelSideTex = material.GetTexture("_RedChannelSideTex");
                _RedChannelSmoothness = material.GetFloat("_RedChannelSmoothness");
                _RedChannelSpecularStrength = material.GetFloat("_RedChannelSpecularStrength");//
                _RedChannelSpecularExponent = material.GetFloat("_RedChannelSpecularExponent");//
                _RedChannelBias = material.GetFloat("_RedChannelBias");
                _GreenChannelTex = material.GetTexture("_GreenChannelTex");
                _GreenChannelSmoothness = material.GetFloat("_GreenChannelSmoothness");
                _GreenChannelSpecularStrength = material.GetFloat("_GreenChannelSpecularStrength");//
                _GreenChannelSpecularExponent = material.GetFloat("_GreenChannelSpecularExponent");//
                _GreenChannelBias = material.GetFloat("_GreenChannelBias");
                _BlueChannelTex = material.GetTexture("_BlueChannelTex");
                _BlueChannelSmoothness = material.GetFloat("_BlueChannelSmoothness");
                _BlueChannelSpecularStrength = material.GetFloat("_BlueChannelSpecularStrength");//
                _BlueChannelSpecularExponent = material.GetFloat("_BlueChannelSpecularExponent");//
                _BlueChannelBias = material.GetFloat("_BlueChannelBias");
                _SnowOn = material.IsKeywordEnabled("MICROFACET_SNOW");
            }
        }

        public void Update()
        {
            
            if (material)
            {
                if(material.name != MaterialName && renderer)
                {
                    GrabMaterialValues();
                    MaterialControllerComponents.PutMaterialIntoMeshRenderer(renderer, material);
                }

                material.SetColor("_Color", _Color);

                material.SetFloat("_NormalStrength", _NormalStrength);

                if (_NormalTex)
                {
                    material.SetTexture("_NormalText", _NormalTex);
                }
                else
                {
                    material.SetTexture("_NormalTex", null);
                }

                material.SetFloat("_RampInfo", Convert.ToSingle(_RampInfo));
                material.SetFloat("_DecalLayer", Convert.ToSingle(_DecalLayer));

                if (_RedChannelTopTex)
                    material.SetTexture("_RedChannelTopTex", _RedChannelTopTex);
                else
                    material.SetTexture("_RedChannelTopTex", null);
                if (_RedChannelSideTex)
                    material.SetTexture("_RedChannelSideTex", _RedChannelSideTex);
                else
                    material.SetTexture("_RedChannelSideTex", null);
                material.SetFloat("_RedChannelSmoothness", _RedChannelSmoothness);
                material.SetFloat("_RedChannelSpecularStrength", _RedChannelSpecularStrength);
                material.SetFloat("_RedChannelSpecularExponent", _RedChannelSpecularExponent);
                material.SetFloat("_RedChannelBias", _RedChannelBias);

                if (_GreenChannelTex)
                    material.SetTexture("_GreenChannelTex", _GreenChannelTex);
                else
                    material.SetTexture("_GreenChannelTex", null);
                material.SetFloat("_GreenChannelSmoothness", _GreenChannelSmoothness);
                material.SetFloat("_GreenChannelSpecularStrength", _GreenChannelSpecularStrength);
                material.SetFloat("_GreenChannelSpecularExponent", _GreenChannelSpecularExponent);
                material.SetFloat("_GreenChannelBias", _GreenChannelBias);

                if (_BlueChannelTex)
                    material.SetTexture("_BlueChannelTex", _BlueChannelTex);
                else
                    material.SetTexture("_BlueChannelTex", null);
                material.SetFloat("_BlueChannelSmoothness", _BlueChannelSmoothness);
                material.SetFloat("_BlueChannelSpecularStrength", _BlueChannelSpecularStrength);
                material.SetFloat("_BlueChannelSpecularExponent", _BlueChannelSpecularExponent);
                material.SetFloat("_BlueChannelBias", _BlueChannelBias);


                material.SetFloat("_Cull", Convert.ToSingle(_Cull));
                material.SetFloat("_TextureFactor", _TextureFactor);
                material.SetFloat("_Depth", _Depth);

                if (_SplatmapTex)
                    material.SetTexture("_SplatmapTex", _SplatmapTex);
                else
                    material.SetTexture("_SplatmapTex", null);

                MaterialControllerComponents.SetShaderKeywordBasedOnBool(_SnowOn, material, "MICROFACET_SNOW");
                MaterialControllerComponents.SetShaderKeywordBasedOnBool(_ColorsOn, material, "USE_VERTEX_COLORS");
                MaterialControllerComponents.SetShaderKeywordBasedOnBool(_MixColorsOn, material, "MIX_VERTEX_COLORS");
                MaterialControllerComponents.SetShaderKeywordBasedOnBool(_MaskOn, material, "USE_ALPHA_AS_MASK");
                MaterialControllerComponents.SetShaderKeywordBasedOnBool(_VerticalBiasOn, material, "USE_VERTICAL_BIAS");
                MaterialControllerComponents.SetShaderKeywordBasedOnBool(_DoubleSampleOn, material, "DOUBLESAMPLE");
            }
        }
    }
}