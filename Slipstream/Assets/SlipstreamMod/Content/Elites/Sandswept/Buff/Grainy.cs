using Moonstorm;
using Moonstorm.Components;
using R2API;
using RoR2;
using System.Linq;
using UnityEngine;
using EntityStates.Sandswept;
using EntityStates;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;
using Slipstream.Components;
using KinematicCharacterController;
using RoR2.Projectile;
using Slipstream.Orbs;
using RoR2.Orbs;

namespace Slipstream.Buffs
{
    public class Grainy : BuffBase
    {
        public override BuffDef BuffDef { get; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<BuffDef>("Grainy");
        public static BuffDef buff;
        private static int buffCount = AffixSandswept.buffCount;

        public static Color dangerColor = ColorUtils.ColorRGB(255f, 56f, 99f, 1f);
        public static float endTintAlpha = 70f / 255f;
        public static float endAlphaBoost = 17f;
        public static float endScrollSpeed = 0.6f;
        //0.33 -> 1
        public override void Initialize()
        {
            buff = BuffDef;

            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;

            //On.RoR2.CharacterModel.UpdateOverlays += CharacterModel_UpdateOverlays;
        }

        //This won't give orbs to non-grainy bodies, there are checks in the actual CreateOrb method.
        //If I don't put this here, minions won't be able to give orbs to their leader.
        private void GlobalEventManager_onCharacterDeathGlobal(DamageReport obj)
        {
            CharacterBody victimBody = obj.victimBody;
            if (victimBody && victimBody.HasBuff(AffixSandswept.buff))
            {
                AffixSandswept.CreateOrb(victimBody, obj.attackerBody);
            }
        }

        public class GrainyBehaviour : BaseBuffBodyBehavior/*, IOnKilledOtherServerReceiver*/
        {
            [BuffDefAssociation(useOnClient = true, useOnServer = true)]
            public static BuffDef GetBuffDef() => SlipContent.Buffs.Grainy;
            private TemporaryOverlay temporaryOverlay;

            private int cachedCount = 0;
            private int buffOnBody = 0;

            private Color initialMaterialTint;
            public float initialScrollSpeed;
            public float initialAlphaBoost;

            private float currentTint;
            private float currentAlphaBoost;
            private float currentScrollSpeed;
            private Color tintColor;
            private Vector4 scrollSpeed;
            private CharacterModel model;

            public void Start()
            {
                Transform modelTransform = body.modelLocator.modelTransform; ;
                if (modelTransform)
                {
                    model = modelTransform.GetComponent<CharacterModel>();
                    InitializeOverlay();
                }
                
            }
            public void OnDestroy()
            {
                if (temporaryOverlay)
                    Destroy(temporaryOverlay);
            }
            public void Update()
            {
                

                if (temporaryOverlay)
                {

                    buffOnBody = body.GetBuffCount(buff);
                    

                    //the actual material properties should line up with the values defined at the top of the Grainy Class.

                    if (buffOnBody > 1 && buffOnBody != cachedCount)
                    {
                        cachedCount = buffOnBody; //Small optimization so  this code doesn't run every frame  until there is a change in the buff on the body

                        currentTint = CalculateIntervals(initialMaterialTint.a, endTintAlpha, buffCount, buffOnBody);
                        currentAlphaBoost = CalculateIntervals(initialAlphaBoost, endAlphaBoost, buffCount, buffOnBody);
                        currentScrollSpeed = CalculateIntervals(initialScrollSpeed, endScrollSpeed, buffCount, buffOnBody);

                        if(buffOnBody <= buffCount - AffixSandswept.buffsApplied)
                            tintColor = new Color(initialMaterialTint.r, initialMaterialTint.g, initialMaterialTint.b, currentTint);
                        else
                            tintColor = new Color(dangerColor.r, dangerColor.g, dangerColor.b, currentTint);

                        scrollSpeed = new Vector4(currentScrollSpeed, currentScrollSpeed, currentScrollSpeed, currentScrollSpeed);


                        temporaryOverlay.materialInstance.SetColor("_TintColor", tintColor);
                        //temporaryOverlay.materialInstance.SetFloat("_ExternalAlpha", currentAlphaBoost);
                        temporaryOverlay.materialInstance.SetFloat("_AlphaBoost", currentAlphaBoost);
                        temporaryOverlay.materialInstance.SetVector("_CutoffScroll", scrollSpeed);
                    }
                }
                else
                {
                    InitializeOverlay();
                }
                
            }
            
            private void InitializeOverlay()
            {
                temporaryOverlay = new TemporaryOverlay();
                temporaryOverlay = body.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.destroyComponentOnEnd = false;
                temporaryOverlay.destroyObjectOnEnd = false;
                temporaryOverlay.originalMaterial = SlipAssets.Instance.MainAssetBundle.LoadAsset<Material>("matEliteSandOverlay");
                initialMaterialTint = temporaryOverlay.originalMaterial.GetColor("_TintColor");
                initialAlphaBoost = temporaryOverlay.originalMaterial.GetFloat("_AlphaBoost");
                initialScrollSpeed = temporaryOverlay.originalMaterial.GetVector("_CutoffScroll").x;
                temporaryOverlay.animateShaderAlpha = false;
                temporaryOverlay.inspectorCharacterModel = model;
            }

            private float CalculateIntervals(float beginning, float end, float maximumBuffs, float buffsActive)
            {
                return ((end - beginning) / (maximumBuffs - 1)) * (buffsActive - 1) + beginning;
            }

            /*public void OnKilledOtherServer(DamageReport damageReport)
            {
                CharacterBody victimBody = damageReport.victimBody;
                if(victimBody && victimBody.HasBuff(AffixSandswept.buff))
                {
                    AffixSandswept.CreateOrb(victimBody, body);
                }
            }*/
            
        }
    }
}
