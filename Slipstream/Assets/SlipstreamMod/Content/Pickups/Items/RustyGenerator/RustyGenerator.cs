using MSU;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;
using R2API;
using RoR2.Items;

namespace Slipstream.Items
{
    public class RustyGenerator : ItemBase
    {
        private const string TOKEN = "SLIP_ITEM_RUSTYGEN_DESC";
        public override ItemDef ItemDef { get; } = SlipAssets.LoadAsset<ItemDef>("RustyGenerator", SlipBundle.Items);

        //baseShield configurable field
        [ConfigureField(ConfigNameOverride = "Base Shield", ConfigDescOverride = "Shield percentage after having at least one stack.", ConfigSectionOverride = "RustyGenerator")]
        [FormatToken(TOKEN, StatTypes.MultiplyByN, 0, "100")]
        public static float baseShield = 0.05f;

        //debuffDurationReduction configurable field
        [ConfigureField(ConfigNameOverride = "Debuff Duration Reduction", ConfigDescOverride = "Percentage of debuff time reduced by one item.", ConfigSectionOverride = "RustyGenerator")]
        [FormatToken(TOKEN, StatTypes.MultiplyByN, 1, "100")]
        public static float debuffDurationReduction = 0.2f; //Og 0.12

        //Hooks! We love hooks :)
        public override void Initialize()
        {
            base.Initialize();
            On.RoR2.CharacterBody.AddTimedBuff_BuffDef_float += CharacterBody_AddTimedBuff_BuffDef_float;
            On.RoR2.CharacterBody.AddTimedBuff_BuffDef_float_int += CharacterBody_AddTimedBuff_BuffDef_float_int;
            On.RoR2.DotController.InflictDot_refInflictDotInfo += DotController_InflictDot_refInflictDotInfo;
        }

        //Hook onto Damage over Time Controller.
        private void DotController_InflictDot_refInflictDotInfo(On.RoR2.DotController.orig_InflictDot_refInflictDotInfo orig, ref InflictDotInfo inflictDotInfo)
        {
            //sets the victimObject (inflicted character) to a characterBody var to use characterBody methods.
            CharacterBody victimBody = inflictDotInfo.victimObject.GetComponent<CharacterBody>();
            //checks if victimBody has an inventory
            if (victimBody.inventory)
            {
                //sets the itemcount of Rusty Generator to a variable count for calculations
                int count = victimBody.inventory.GetItemCount(ItemDef);
                RustyGeneratorBehavior component = victimBody.GetComponent<RustyGeneratorBehavior>();
                //checks if the itemcount is greater than 0, and if the victimBody has shield active
                if (count > 0 && (victimBody.healthComponent.shield > 0 || component.triggerReduction))
                {
                    Util.PlaySound("Play_huntress_R_snipe_shoot", victimBody.gameObject);
                    component.triggerReduction = false;
                    //reduces the duration of the DoT using MSU's InverseHyperbolicScaling method for calculation
                    inflictDotInfo.duration -= (inflictDotInfo.duration * MSUtil.InverseHyperbolicScaling(debuffDurationReduction, debuffDurationReduction, 1, count));

                    //For things like burn, ignition tank burn, and helfire. Total damage is directly tied with the duration of these Dots, inflictDotInfo.duration doesn't effect these Dots.
                    //This shouldn't affect Dots like bleed.
                    if(inflictDotInfo.totalDamage != null)
                        inflictDotInfo.totalDamage -= (inflictDotInfo.totalDamage * MSUtil.InverseHyperbolicScaling(debuffDurationReduction, debuffDurationReduction, 1, count));
                }
            }
            //inserts modified parameters back into the original method
            orig(ref inflictDotInfo);
        }

        //Hook onto Overloaded AddTimedBuff with maxStacks
        private void CharacterBody_AddTimedBuff_BuffDef_float_int(On.RoR2.CharacterBody.orig_AddTimedBuff_BuffDef_float_int orig, CharacterBody self, BuffDef buffDef, float duration, int maxStacks)
        {
            //checks if the characterBody has an inventory
            if (self.inventory)
            {
                //sets the itemcount of Rusty Generator to a variable count for calculations
                int count = self.inventory.GetItemCount(ItemDef);
                RustyGeneratorBehavior component = self.GetComponent<RustyGeneratorBehavior>();
                //checks if the itemcount is greater than 0, checks if the newly applied buff is a debuff, checks for the stack count of said debuff, and if the characterBody has shield active or had shield before the instance of damage
                if (count > 0 && buffDef.isDebuff && self.GetBuffCount(buffDef) < maxStacks && (self.healthComponent.shield > 0 || component.triggerReduction))
                {
                    Util.PlaySound("Play_huntress_R_snipe_shoot", self.gameObject);
                    //reduces the duration of the debuff using MSU's InverseHyperbolicScaling method for calculation
                    duration -= (duration * MSUtil.InverseHyperbolicScaling(debuffDurationReduction, debuffDurationReduction, 1, count));
                    component.triggerReduction = false;
                }
            }
            //inserts modified parameters back into the original method
            orig(self, buffDef, duration, maxStacks);
        }

        //Hook onto Overloaded AddTimedBuff WITHOUT maxStacks
        private void CharacterBody_AddTimedBuff_BuffDef_float(On.RoR2.CharacterBody.orig_AddTimedBuff_BuffDef_float orig, CharacterBody self, BuffDef buffDef, float duration)
        {
            //checks if the characterBody has an inventory
            if (self.inventory)
            {
                //sets the itemcount of Rusty Generator to a variable count for calculations
                int count = self.inventory.GetItemCount(ItemDef);
                RustyGeneratorBehavior component = self.GetComponent<RustyGeneratorBehavior>();
                //checks if the itemcount is greater than 0, checks if the newly applied buff is a debuff, and if the characterBody has shield active or had shield before the instance of damage
                if (count > 0 && buffDef.isDebuff && (self.healthComponent.shield > 0 || component.triggerReduction))
                {
                    Util.PlaySound("Play_huntress_R_snipe_shoot", self.gameObject);
                    //reduces the duration of the debuff using MSU's InverseHyperbolicScaling method for calculation
                    duration -= (duration * MSUtil.InverseHyperbolicScaling(debuffDurationReduction, debuffDurationReduction, 1, count));
                    component.triggerReduction = false;
                }
            }
            //inserts modified parameters back into the original method
            orig(self, buffDef, duration);
        }

        public class RustyGeneratorBehavior : BaseItemBodyBehavior, IBodyStatArgModifier, IOnIncomingDamageServerReceiver, IOnTakeDamageServerReceiver
        {
            [ItemDefAssociation(useOnClient = true, useOnServer = true)]
            public static ItemDef GetItemDef() => SlipContent.Items.RustyGenerator;

            private bool hadShield = false;
            public bool triggerReduction = false;

            /*public void Awake()
            {
                body.RecalculateStats();
            }*/
            public void ModifyStatArguments(RecalculateStatsAPI.StatHookEventArgs args)
            {
                //adds 3% of max health as shield to the player
                args.baseShieldAdd += body.healthComponent.fullHealth * baseShield;
            }

            //checks before and after damage to see if the body had shield before that instance of damage
            public void OnIncomingDamageServer(DamageInfo damageInfo)
            {
                if (body.healthComponent.shield > 0f)
                    hadShield = true;
                else
                    hadShield = false;
            }

            public void OnTakeDamageServer(DamageReport damageReport)
            {
                if (body.healthComponent.shield <= 0f && hadShield)
                    triggerReduction = true;
                else
                    triggerReduction = false;
            }
        }
    }
}
