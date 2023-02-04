using Moonstorm;
using UnityEngine.AddressableAssets;
using Slipstream.Buffs;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;
using R2API;
using On.RoR2.UI;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using RoR2.Items;
using System.Linq;


namespace Slipstream.Items
{

    public class BlackHealth : ItemBase
    {
        public override ItemDef ItemDef { get; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<ItemDef>("BlackHealth");
        private static Material material = SlipAssets.Instance.MainAssetBundle.LoadAsset<Material>("matBlackBlood");

        public static float armorIncrease = Coalition.armorIncrease;

        private static bool preventsVoidDeath = Coalition.preventsVoidDeath;

        public static Color blackOverHealthColor = ColorUtils.ColorRGB(11f, 11f, 9f);
        public static Color blackHealingHealthColor = ColorUtils.ColorRGB(159f, 160, 159f);

        public override void Initialize()
        {
            On.RoR2.UI.HealthBar.UpdateBarInfos += new On.RoR2.UI.HealthBar.hook_UpdateBarInfos(HealthBar_UpdateBarInfos);
        }

        private void HealthBar_UpdateBarInfos(On.RoR2.UI.HealthBar.orig_UpdateBarInfos orig, RoR2.UI.HealthBar self)
        {
            orig(self);
            CharacterBody characterBody = self.source?.body;
            if (characterBody)
            {
                Inventory inventory = characterBody.inventory;
                if (inventory && inventory.GetItemCount(ItemDef) > 0)
                {
                    //going to change the visuals of this soon
                    self.barInfoCollection.trailingOverHealthbarInfo.color = blackOverHealthColor;
                    self.barInfoCollection.instantHealthbarInfo.color = blackHealingHealthColor;
                }
            }
        }


        public class BlackHealthBehavior : BaseItemBodyBehavior, IBodyStatArgModifier
        {
            [ItemDefAssociation(useOnClient = true, useOnServer = true)]

            public static RoR2.ItemDef GetItemDef() => SlipContent.Items.BlackHealth;

            CharacterMaster allyOwnerMaster;

            private bool changedFlag;

            private TemporaryOverlay overlay;
            
            private CharacterModel model;

            public void OnEnable()
            {
                //Makes owners of Blackhealth completely immune to void explosions (when its enabled)
                changedFlag = false;
                allyOwnerMaster = body.master?.minionOwnership.ownerMaster;
                if ((body.bodyFlags & CharacterBody.BodyFlags.ImmuneToVoidDeath) == CharacterBody.BodyFlags.None && preventsVoidDeath)
                {
                    changedFlag = true;
                    body.bodyFlags |= CharacterBody.BodyFlags.ImmuneToVoidDeath;
                }
                model = body.modelLocator.modelTransform.GetComponent<CharacterModel>();
                if (model)
                    AddBlackOverlay();

            }

            public void OnDisable()
            {
                //Removes tag if the immunity was given (that way allies somehow initially having the ImmuneToVoidDeath flag don't get screwed over)
                if (changedFlag)
                    body.bodyFlags &= ~CharacterBody.BodyFlags.ImmuneToVoidDeath;
                if (overlay)
                {
                    Destroy(overlay);
                    overlay = null;
                }

            }

            public void FixedUpdate()
            {
                if (NetworkServer.active && !allyOwnerMaster)
                    body.inventory?.ResetItem(SlipContent.Items.BlackHealth);
                if (model && !overlay)
                    AddBlackOverlay();
            }

            public void ModifyStatArguments(RecalculateStatsAPI.StatHookEventArgs args)
            {
                if (allyOwnerMaster)
                    args.armorAdd += allyOwnerMaster.inventory.GetItemCount(SlipContent.Items.Coalition) * armorIncrease;
            }

            public void AddBlackOverlay()
            {
                overlay = new TemporaryOverlay();
                overlay = body.gameObject.AddComponent<TemporaryOverlay>();
                overlay.duration = 0f;
                overlay.destroyComponentOnEnd = false;
                overlay.destroyObjectOnEnd = false;
                overlay.originalMaterial = material;
                overlay.inspectorCharacterModel = model;
            }
        }
    }
}