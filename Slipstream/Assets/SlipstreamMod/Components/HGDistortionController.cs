using System.Collections;
using UnityEngine;
using Moonstorm.Components;
using System;

namespace Slipstream.Components
{
    public class HGDistortionController : MaterialControllerComponents.MaterialController
    {
        public Texture _BumpMap;
        public Texture _MaskTex;

        [Range(0f, 10f)]
        public float _Magnitude;

        
        public float _NearFadeZeroDistance;
        public float _NearFadeOneDistance;
        public float _FarFadeZeroDistance;
        public float _FarFadeOneDistance;

        public bool _DistanceModulationOn;

        [Range(0f, 1f)]
        public float _DistanceModulationMagnitude;

        [Range(0f, 2f)]
        public float _InvFade;

        public void Start()
        {
            GrabMaterialValues();
        }
        public void GrabMaterialValues()
        {
            if (material)
            {
                _BumpMap = material.GetTexture("_BumpMap");
                _MaskTex = material.GetTexture("_MaskTex");
                _Magnitude = material.GetFloat("_Magnitude");
                _NearFadeZeroDistance = material.GetFloat("_NearFadeZeroDistance");
                _NearFadeOneDistance = material.GetFloat("_NearFadeOneDistance");
                _FarFadeZeroDistance = material.GetFloat("_FarFadeZeroDistance");
                _FarFadeOneDistance = material.GetFloat("_FarFadeOneDistance");
                _DistanceModulationOn = material.IsKeywordEnabled("DISTANCEMODULATION");
                _DistanceModulationMagnitude = material.GetFloat("_DistanceModulationMagnitude");
                _InvFade = material.GetFloat("_InvFade");
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

                if (_BumpMap)
                    material.SetTexture("_BumpMap", _BumpMap);
                else
                    material.SetTexture("_BumpMap", _BumpMap);

                if (_MaskTex)
                    material.SetTexture("_MaskTex", _MaskTex);
                else
                    material.SetTexture("_MaskTex", _MaskTex);

                material.SetFloat("_Magnitude", _Magnitude);
                material.SetFloat("_NearFadeZeroDistance", _NearFadeZeroDistance);
                material.SetFloat("_NearFadeOneDistance", _NearFadeOneDistance);
                material.SetFloat("_FarFadeZeroDistance", _FarFadeZeroDistance);
                material.SetFloat("_FarFadeOneDistance", _FarFadeOneDistance);

                MaterialControllerComponents.SetShaderKeywordBasedOnBool(_DistanceModulationOn, material, "DISTANCEMODULATION");
                material.SetFloat("_DistanceModulationMagnitude", _DistanceModulationMagnitude);
                material.SetFloat("_InvFade", _InvFade);
            }
        }

    }
}