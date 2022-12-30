﻿using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using Slipstream;
using Slipstream.Buffs;
using System.Collections.Generic;
using AK;
namespace EntityStates.Sandswept
{
    public class GlassState : BaseState
    {
        private TemporaryOverlay temporaryOverlay;
        private Animator modelAnimator;
        private AimAnimator aimAnimator;
        private AkEvent[] AkSoundEvent;
        private AkEvent[] AkSoundEventChildren;
        private ParticleSystem[] particleSystem;
        private float duration;
        private float freezeDuration = 30f;

        public Color beginColor = new Color(255f, 255f, 255f, 255f);
        public Color endColor = new Color(94f, 255f, 236f, 255f);

        private List<Material> instancesOverlay;
        private Material instanceIndicator;
        public Material frozenOverlayMaterial = SlipAssets.Instance.MainAssetBundle.LoadAsset<Material>("matIsGlass");
        public Material frozenFlashMaterial = SlipAssets.Instance.MainAssetBundle.LoadAsset<Material>("matIsGlassGradient");

        private GameObject eliteSandKnockbackIndicator;

        private bool indicatorEnabled
        {
            get
            {
                return eliteSandKnockbackIndicator;
            }
            set
            {
                if(indicatorEnabled == value)
                {
                    return;
                }
                if (value)
                {
                    
                    GameObject original = SlipAssets.Instance.MainAssetBundle.LoadAsset<GameObject>("EliteSandKnockbackIndicator");
                    //ref int pos;
                    //Vector3 corePosition = RoR2.Util.GetCorePosition(characterBody.gameObject);
                    float diameter = AffixSandswept.CalculateRadius(characterBody) * 2f;

                    eliteSandKnockbackIndicator = UnityEngine.Object.Instantiate<GameObject>(original, characterBody.footPosition, Quaternion.identity);
                    eliteSandKnockbackIndicator.transform.localScale = new Vector3(diameter, diameter, diameter);
                    eliteSandKnockbackIndicator.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(base.gameObject, null);

                    MeshRenderer renderer = eliteSandKnockbackIndicator.transform.Find("Radius, Spherical").GetComponent<MeshRenderer>();
                    if (renderer)
                        instanceIndicator = renderer.material;
                    return;
                }

            }
        }

        //private bool died;

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
        public override void OnEnter()
        {
            base.OnEnter();
            /*if (sfxLocator && sfxLocator.barkSound != "")
                Util.PlaySound(base.sfxLocator.barkSound, base.gameObject);*/
            Transform modelTransform = GetModelTransform();
            if (modelTransform)
            {
                CharacterModel model = modelTransform.GetComponent<CharacterModel>();

                if (model)
                {
                    /*temporaryOverlay = gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay.duration = freezeDuration; //doesn't matter what you put here
                    temporaryOverlay.destroyComponentOnEnd = false;
                    temporaryOverlay.destroyObjectOnEnd = false;
                    temporaryOverlay.originalMaterial = frozenFlashMaterial;
                    temporaryOverlay.AddToCharacerModel(model);*/

                    //sets the material of the mf to be glass
                    for(int i = 0; i < model.baseRendererInfos.Length; i++)
                    {
                        var mat = model.baseRendererInfos[i].defaultMaterial;
                        if(mat.shader.name.StartsWith("Hopoo Games/Deferred"))
                        {
                            mat = frozenOverlayMaterial;
                            model.baseRendererInfos[i].defaultMaterial = mat;
                            //instancesOverlay.Add(model.baseRendererInfos[i].defaultMaterial);
                        }
                    }

                    model.particleMaterialOverride = frozenOverlayMaterial;
                    model.lightColorOverride = new Color32(0, 0, 0, 0);

                    //turn off all lights
                    CharacterModel.LightInfo[] lights = model.baseLightInfos;
                    for(int i = 0; i < lights.Length; i++)
                    {
                        lights[i].light.gameObject.SetActive(false);
                    }

                    //model.material
                }
            }
            modelAnimator = base.GetModelAnimator();
            //Freezes animations
            if (modelAnimator)
            {
                //modelAnimator.enabled = false;
                modelAnimator.speed = 0; //this should have the same effect as modelAnimator.enabled = false but without fucking up the IKs on Beetle Queen and Void Reavers
                aimAnimator = base.GetAimAnimator();
                if (aimAnimator)
                    aimAnimator.enabled = false;
                duration = freezeDuration;
            }
            if(rigidbody && !rigidbody.isKinematic)
            {
                rigidbody.velocity = Vector3.zero;
                if (rigidbodyMotor)
                    rigidbodyMotor.moveVector = Vector3.zero;
            }
            if (characterDirection)
            {
                //characterDirection.moveVector = characterDirection.forward;
                characterDirection.enabled = false;
            }
            //Destroys any sound emitters
            AkSoundEvent = characterBody.gameObject.GetComponents<AkEvent>();
            if(AkSoundEvent.Length > 0)
            {
                for (int i = 0; i < AkSoundEvent.Length; i++)
                    Destroy(AkSoundEvent[i]);

            }
            AkSoundEventChildren = characterBody.gameObject.GetComponentsInChildren<AkEvent>();
            if (AkSoundEventChildren.Length > 0)
            {
                for (int i = 0; i < AkSoundEventChildren.Length; i++)
                    Destroy(AkSoundEventChildren[i]);

            }
            //Destroys any particle systems (Wisp flame turns off for example)
            particleSystem = characterBody.gameObject.GetComponentsInChildren<ParticleSystem>();
            if(particleSystem.Length > 0)
            {
                for (int i = 0; i < particleSystem.Length; i++)
                    Destroy(particleSystem[i]);
            }

            indicatorEnabled = true;
            
        }


        public override void OnExit()
        {
            /*if (temporaryOverlay)
                Destroy(temporaryOverlay);*/

            //gameObject.GetComponent<AffixSandswept.AffixSandsweptBehavior>().isGlass = false;
            //EffectManager.SpawnEffect()
            if(eliteSandKnockbackIndicator)
                Destroy(eliteSandKnockbackIndicator);

            CharacterBody body = base.characterBody;
            if (body)
            {
                Transform transform = body.modelLocator.modelTransform;
                if (transform)
                {
                    EntityState.Destroy(transform.gameObject);
                    transform = null;
                }
            }
            
            indicatorEnabled = false;

            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (NetworkServer.active)
            {
                if (instanceIndicator)
                    instanceIndicator.color = Color.Lerp(beginColor, endColor, fixedAge);

                /*
                for (int i = 0; i < instancesOverlay.Count; i++)
                {
                    instancesOverlay[i].color = Color.Lerp(beginColor, endColor, fixedAge);
                }*/

                if (base.fixedAge >= duration)
                {
                    AffixSandswept.FireKBBlast(characterBody);
                    base.characterBody.healthComponent.Suicide();
                }
                    
            }

        }        
    }
}