/*using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Slipstream.Items
{
    public class RottingPerforator
    {
        public static ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();

        public void Init() //This runs whatever methods you need to for the item, such as language stuff, effects, setting up the item, etc. Init is ran in the Awake() method in the Main.cs file and every item you add you have to run Init() inside of Awake() in the Main.cs file or else it won't work.
        {
            //RottingPerforatorItem();
            AddLanguageTokens();
            Hooks();
        }

        private void RottingPerforatorItem()
        {
            itemDef.name = "RottingPerforator"; //Internal name of the item
            itemDef.tier = ItemTier.Boss; //Tier of the item, Tier1 = White. Tier2 = Green, Tier3 = Red. Lunar = Blue. Boss = Yellow
            itemDef.pickupModelPrefab = SlipstreamPlugin.AssetBundle.LoadAsset<GameObject>("Assets/Import/RottingPerforator/rottingperforator.prefab"); //3D Model of the item in the world/Logbook
            itemDef.pickupIconSprite = SlipstreamPlugin.AssetBundle.LoadAsset<Sprite>("Assets/Import/RottingPerforator/rottingperforator_icon.png"); //Sprite of the item  when in your inventory/Logbook
            itemDef.nameToken = "ROTTINGPERFORATOR_NAME"; //Name of the item ingame
            itemDef.pickupToken = "ROTTINGPERFORATOR_PICKUP"; //Text when you hover over the item in your inventoy or the tooltip when you pick it up
            itemDef.descriptionToken = "ROTTINGPERFORATOR_DESC"; //Description of the item in the logbook (also I think what the tooltip is with ItemStats on?
            itemDef.loreToken = "ROTTINGPERFORATOR_LORE"; //Lore of the item in the logbook
            itemDef.tags = new ItemTag[] { ItemTag.Damage, ItemTag.OnKillEffect }; //Tags for the item, for stuff such as classifying it for Utility/Damage/Healing chests, if it's blacklisted from Mithrix, if it's scrap, if it's sprint related, etc.

            //TODO: MAKE THIS SHIT ACTUALLY WORK
            var itemDisplayRules = new ItemDisplayRule[1];
            itemDisplayRules[0].followerPrefab = SlipstreamPlugin.AssetBundle.LoadAsset<GameObject>("Assets/Import/snake/snake.prefab");
            itemDisplayRules[0].childName = "Chest";
            itemDisplayRules[0].localScale = new Vector3(2f, 2f, 2f);

            SlipstreamPlugin.ModItemDefs.Add(itemDef); //Adds this item to the list of itemDefs in Main.cs, which then gets added to the ContentPack which then gets added to the game's content
        }

        private void AddLanguageTokens()
        {
            LanguageAPI.Add("ROTTINGPERFORATOR_NAME", "Rotting Perforator"); //Name of the item ingame
            LanguageAPI.Add("ROTTINGPERFORATOR_PICKUP", "Chance to create a Malachite Urchin on enemy kill."); //Text when you hover over the item in your inventoy or the tooltip when you pick it up
            LanguageAPI.Add("ROTTINGPERFORATOR_DESC", "<style=cIsDamage>10%</style> chance on killing an enemy to create a friendly Malachite Urchin that attacks nearby enemies. The Malachite Urchin triggers any on-kill effects when it dies <style=cIsUtility>1</style> <style=cStack>(+1 per stack)</style> times."); //Description of the item in the logbook (also I think what the tooltip is with ItemStats on?
            LanguageAPI.Add("ROTTINGPERFORATOR_LORE", "Subject: Rotting Perforator\n\nTechnician: Dema \"Dembones\" Brown\nTable Spec: N/A\nNotes:\n\n> I am NOT touching this thing.\n> Prepare for immediate disposal, this thing is absolutely vile. Not worth experimenting on."); //Lore of the item in the logbook. \n creates a new line.
        }
        public static TeamIndex? teamIndex = new TeamIndex?(TeamIndex.Player);
        private void Hooks()
        {
            //Spawning Urchins
            On.RoR2.GlobalEventManager.OnCharacterDeath += (On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport) =>
            {
                if (damageReport?.attackerBody && damageReport?.attackerMaster)
                {
                    CharacterBody character = damageReport.attackerBody;
                    if (character.inventory.GetItemCount(itemDef) > 0 && Util.CheckRoll(10f, damageReport.attackerMaster) && damageReport.victimBody.inventory.GetItemCount(MalachiteUrchinOnKill.itemDef) == 0)
                    {
                        InputBankTest inputBankTest = null;
                        Ray ray = inputBankTest ? inputBankTest.GetAimRay() : new Ray(damageReport.victimBody.transform.position, damageReport.victimBody.transform.rotation * Vector3.forward);
                        Vector3 vector = damageReport.victimBody.transform.position;
                        Vector3 position2 = vector;
                        Quaternion rotation2 = Quaternion.LookRotation(ray.direction);
                        GameObject gameObject3 = Object.Instantiate(Resources.Load<GameObject>("Prefabs/CharacterMasters/UrchinTurretMaster"), position2, rotation2);
                        CharacterMaster component2 = gameObject3.GetComponent<CharacterMaster>();
                        NetworkServer.Spawn(gameObject3);
                        MinionOwnership.MinionGroup.SetMinionOwner(component2.minionOwnership, damageReport.attackerMaster.netId);
                        component2.teamIndex = (TeamIndex)teamIndex;
                        component2.inventory.GiveItem(MalachiteUrchinOnKill.itemDef, character.inventory.GetItemCount(itemDef));
                        component2.inventory.GiveItem(RoR2Content.Items.HealthDecay, 1);
                        component2.inventory.GiveItem(RoR2Content.Items.BoostAttackSpeed, 10 * character.inventory.GetItemCount(itemDef) - 1);
                        component2.SpawnBodyHere();
                    }
                }
                orig(self, damageReport);
            };
            
            //Urchins proccing on kill items when they die
            On.RoR2.CharacterBody.OnDeathStart += (orig, characterBody) =>
            {
                if (characterBody.master.teamIndex == teamIndex && characterBody.inventory.GetItemCount(MalachiteUrchinOnKill.itemDef) > 0)
                {
                    if (NetworkServer.active)
                    {
                        for (int i = 0; i < characterBody.inventory.GetItemCount(MalachiteUrchinOnKill.itemDef); i++)
                        {
                            DamageInfo damageInfo = new DamageInfo
                            {
                                attacker = characterBody.master.minionOwnership.ownerMaster.GetBodyObject(),
                                crit = false,
                                damage = characterBody.damage,
                                position = characterBody.transform.position,
                                procCoefficient = 1f,
                                damageType = DamageType.Generic,
                                damageColorIndex = DamageColorIndex.Default
                            };
                            HealthComponent victim = characterBody.healthComponent;
                            DamageReport damageReport = new DamageReport(damageInfo, victim, damageInfo.damage, characterBody.healthComponent.combinedHealth);
                            characterBody.HandleOnKillEffectsServer(damageReport); 
                            GlobalEventManager.instance.OnCharacterDeath(damageReport);
                        }
                    }
                }
                orig(characterBody);
            };
        }
    }
}*/