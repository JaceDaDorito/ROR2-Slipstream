using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoR2;
using EntityStates;
using UnityEngine.Networking;
using RoR2.Orbs;
using Slipstream.Buffs;
using UnityEngine.Networking;

namespace EntityStates.Sandswept
{
    public class SandsweptGlassShatter : GenericCharacterDeath
    {
        public Run.FixedTimeStamp explodeBuffer;
        public override void OnEnter()
        {
            if (cachedModelTransform)
            {
                explodeBuffer = Run.FixedTimeStamp.now;
                TemporaryOverlay temporaryOverlay = this.cachedModelTransform.gameObject.AddComponent<TemporaryOverlay>();

                temporaryOverlay.duration = 0.1f;
                temporaryOverlay.destroyObjectOnEnd = true;
                temporaryOverlay.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matShatteredGlass");
                temporaryOverlay.inspectorCharacterModel = this.cachedModelTransform.gameObject.GetComponent<CharacterModel>();
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 0f, 0.1f, 1f);
                temporaryOverlay.animateShaderAlpha = true;

                if (cameraTargetParams)
                {
                    ChildLocator component = cachedModelTransform.GetComponent<ChildLocator>();
                    if (component)
                    {
                        Transform transform = component.FindChild("Chest");
                        if (transform)
                        {
                            cameraTargetParams.cameraPivotTransform = transform;
                            aimRequest = cameraTargetParams.RequestAimType(CameraTargetParams.AimType.Aura);
                            cameraTargetParams.dontRaycastToPivot = true;
                        }
                    }
                }
            }
                
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(cachedModelTransform && explodeBuffer.timeSince > 0.1f)
            {
                EntityState.Destroy(cachedModelTransform.gameObject);
                cachedModelTransform = null;
            }
        }
    }
}