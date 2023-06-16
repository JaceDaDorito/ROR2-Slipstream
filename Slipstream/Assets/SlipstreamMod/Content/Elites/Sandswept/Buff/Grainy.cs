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
        public override BuffDef BuffDef { get; } = SlipAssets.LoadAsset<BuffDef>("Grainy", SlipBundle.Elites);
        private static Material eliteSandOverlay = SlipAssets.LoadAsset<Material>("matEliteSandOverlay", SlipBundle.Elites);
        public static BuffDef buff;
        private static int buffCount = AffixSandswept.buffCount;

        public static Color dangerColor = SlipUtils.ColorRGB(255f, 56f, 99f, 1f);
        public static float endTintAlpha = 70f / 255f;
        public static float endAlphaBoost = 17f;
        public static float endScrollSpeed = 0.6f;
        //0.33 -> 1
        public override void Initialize()
        {
            buff = BuffDef;
        }


        public class GrainyBehaviour : BaseBuffBodyBehavior/*, IOnKilledOtherServerReceiver*/
        {
            [BuffDefAssociation(useOnClient = true, useOnServer = true)]
            public static BuffDef GetBuffDef() => SlipContent.Buffs.Grainy;
            private TemporaryOverlay temporaryOverlay = null;

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
                if (temporaryOverlay != null)
                    Destroy(temporaryOverlay);
            }
            public void Update()
            {
                
                if (temporaryOverlay != null)
                {
                    //SlipLogger.LogD("Start of Overlay");
                    buffOnBody = body.GetBuffCount(buff);

                    if (buffOnBody > 1 && buffOnBody != cachedCount && temporaryOverlay.materialInstance != null)
                    {
                        //SlipLogger.LogD("Start of overlay creation");
                        cachedCount = buffOnBody; //Small optimization so  this code doesn't run every frame  until there is a change in the buff on the body

                        currentTint = CalculateIntervals(initialMaterialTint.a, endTintAlpha, buffCount, buffOnBody);
                        currentAlphaBoost = CalculateIntervals(initialAlphaBoost, endAlphaBoost, buffCount, buffOnBody);
                        currentScrollSpeed = CalculateIntervals(initialScrollSpeed, endScrollSpeed, buffCount, buffOnBody);

                        //SlipLogger.LogD("value finding");
                        if (buffOnBody <= buffCount - AffixSandswept.buffsApplied)
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
                    //SlipLogger.LogD("Overlay Reinitialized");
                    InitializeOverlay();
                }
                
            }
            
            private void InitializeOverlay()
            {
                temporaryOverlay = new TemporaryOverlay();
                temporaryOverlay = body.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.destroyComponentOnEnd = false;
                temporaryOverlay.destroyObjectOnEnd = false;
                temporaryOverlay.originalMaterial = eliteSandOverlay;
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
