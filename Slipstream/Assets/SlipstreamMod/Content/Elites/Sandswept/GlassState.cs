using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using Slipstream;
using Slipstream.Buffs;
using System.Collections.Generic;
using AK;
using System.Collections.ObjectModel;

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
        protected float radius;
        protected static float invulnDuration = AffixSandswept.timeInvulnerable;
        private float animatorVel = 0f;

        public static Color beginColor = ColorUtils.ColorRGB(255f, 255f, 255f);
        public static Color endColor = ColorUtils.ColorRGB(168f, 50f, 76f);
        //(94f, 255f, 236f)

        //private List<Material> instancesOverlay;
        private List<CharacterModel.RendererInfo> rendererList = new List<CharacterModel.RendererInfo>();
        public Material frozenOverlayMaterial = SlipAssets.Instance.MainAssetBundle.LoadAsset<Material>("matIsGlass");
        public Material frozenFlashMaterial = SlipAssets.Instance.MainAssetBundle.LoadAsset<Material>("matIsGlassFlash");

        private Color CurrentColor;

        private GameObject eliteSandKnockbackIndicator;

        public CharacterBody attackerBody;

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
                    

                    eliteSandKnockbackIndicator = UnityEngine.Object.Instantiate<GameObject>(original, characterBody.footPosition, Quaternion.identity);
                    eliteSandKnockbackIndicator.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(base.gameObject, null);
                    eliteSandKnockbackIndicator.transform.Find("Radius, Spherical").GetComponent<MeshRenderer>();
                    return;
                }
                UnityEngine.Object.Destroy(eliteSandKnockbackIndicator);
                eliteSandKnockbackIndicator = null;
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
            Util.PlaySound("Play_item_proc_armorReduction_hit", base.gameObject);

            Transform modelTransform = GetModelTransform();
            if (modelTransform)
            {
                CharacterModel model = modelTransform.GetComponent<CharacterModel>();

                if (model)
                {
                    temporaryOverlay = gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay.duration = invulnDuration; //doesn't matter what you put here
                    temporaryOverlay.destroyComponentOnEnd = true;
                    temporaryOverlay.destroyObjectOnEnd = false;
                    temporaryOverlay.originalMaterial = frozenFlashMaterial;
                    temporaryOverlay.inspectorCharacterModel = model;
                    temporaryOverlay.alphaCurve = AnimationCurve.Linear(0f, 1f, invulnDuration, 0f);
                    temporaryOverlay.animateShaderAlpha = true;

                    //sets the material of the mf to be glass
                    for(int i = 0; i < model.baseRendererInfos.Length; i++)
                    {
                        var mat = model.baseRendererInfos[i].defaultMaterial;
                        if(mat.shader.name.StartsWith("Hopoo Games/Deferred"))
                        {
                            mat = frozenOverlayMaterial;
                            model.baseRendererInfos[i].defaultMaterial = mat;
                            rendererList.Add(model.baseRendererInfos[i]);
                        }
                    }

                    //sets the material of the mf's item displays to be glass
                    ItemDisplay[] itemDisplays = modelTransform.GetComponents<ItemDisplay>();
                    if(itemDisplays.Length > 0)
                    {
                        for (int i = 0; i < itemDisplays.Length; i++)
                        {
                            var rendererInfos = itemDisplays[i].rendererInfos;
                            for(int j = 0; j < rendererInfos.Length; i++)
                            {
                                var mat = rendererInfos[i].defaultMaterial;
                                if (mat.shader.name.StartsWith("Hopoo Games/Deferred"))
                                {
                                    mat = frozenOverlayMaterial;
                                    rendererInfos[i].defaultMaterial = mat;
                                    rendererList.Add(rendererInfos[i]);
                                }
                            }
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
                //modelAnimator.speed = 0; //this should have the same effect as modelAnimator.enabled = false but without fucking up the IKs on Beetle Queen and Void Reavers
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

            radius = AffixSandswept.CalculateRadius(characterBody);
            float diameter = radius * 2f;

            indicatorEnabled = true;
            eliteSandKnockbackIndicator.transform.localScale = new Vector3(diameter, diameter, diameter);

        }


        public override void OnExit()
        {
            /*if (temporaryOverlay)
                Destroy(temporaryOverlay);*/

            //gameObject.GetComponent<AffixSandswept.AffixSandsweptBehavior>().isGlass = false;
            //EffectManager.SpawnEffect()
            Util.PlaySound("Play_char_glass_death", base.gameObject);

            if (eliteSandKnockbackIndicator)
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

                if(rendererList.Count > 0)
                {
                    CurrentColor = Color.Lerp(beginColor, endColor, Mathf.Clamp01(fixedAge / freezeDuration));
                    for (int i = 0; i < rendererList.Count; i++)
                    {
                        rendererList[i].defaultMaterial.SetColor("_TintColor", CurrentColor);
                    }
                }
                

                if(fixedAge <= invulnDuration && modelAnimator)
                {
                    modelAnimator.speed = Mathf.SmoothDamp(modelAnimator.speed, 0f, ref animatorVel, invulnDuration/2);
                }
                else if (modelAnimator)
                {
                    modelAnimator.speed = 0f;
                }

                if (base.fixedAge >= duration)
                {
                    CommitSuicide();
                }
                    
            }

        }

        protected void CommitSuicide()
        {
            AffixSandswept.FireKBBlast(characterBody);
            base.characterBody.healthComponent.Suicide(attackerBody?.gameObject); //already defaults to null if theres no attacker body
        }
    }
}