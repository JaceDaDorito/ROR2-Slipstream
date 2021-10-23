/*using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Slipstream.Items
{
    public class SubzeroPerforator
    {
        public static ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();

        public void Init() //This runs whatever methods you need to for the item, such as language stuff, effects, setting up the item, etc. Init is ran in the Awake() method in the Main.cs file and every item you add you have to run Init() inside of Awake() in the Main.cs file or else it won't work.
        {
            //SubzeroPerforatorItem();
            AddLanguageTokens();
            Hooks();
        }

        private void SubzeroPerforatorItem()
        {
            itemDef.name = "SubzeroPerforator"; //Internal name of the item
            itemDef.tier = ItemTier.Boss; //Tier of the item, Tier1 = White. Tier2 = Green, Tier3 = Red. Lunar = Blue. Boss = Yellow
            itemDef.pickupModelPrefab = SlipstreamPlugin.AssetBundle.LoadAsset<GameObject>("Assets/Import/SubzeroPerforator/subzeroperforator.prefab"); //3D Model of the item in the world/Logbook
            itemDef.pickupIconSprite = SlipstreamPlugin.AssetBundle.LoadAsset<Sprite>("Assets/Import/SubzeroPerforator/subzeroperforator_icon.png"); //Sprite of the item  when in your inventory/Logbook
            itemDef.nameToken = "SUBZEROPERFORATOR_NAME"; //Name of the item ingame
            itemDef.pickupToken = "SUBZEROPERFORATOR_PICKUP"; //Text when you hover over the item in your inventoy or the tooltip when you pick it up
            itemDef.descriptionToken = "SUBZEROPERFORATOR_DESC"; //Description of the item in the logbook (also I think what the tooltip is with ItemStats on?
            itemDef.loreToken = "SUBZEROPERFORATOR_LORE"; //Lore of the item in the logbook
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
            LanguageAPI.Add("SUBZEROPERFORATOR_NAME", "Subzero Perforator"); //Name of the item ingame
            LanguageAPI.Add("SUBZEROPERFORATOR_PICKUP", "Chance on enemy kill to create an icy explosion."); //Text when you hover over the item in your inventoy or the tooltip when you pick it up
            LanguageAPI.Add("SUBZEROPERFORATOR_DESC", "On killing an enemy, you have a <style=cIsDamage>10%</style> chance spawn an <style=cIsDamage>ice explosion</style> in a <style=cIsDamage>10m</style> <style=cStack>(+3m per stack)</style> radius for <style=cIsDamage>300%</style> <style=cStack>(+300% per stack)</style> base damage."); //Description of the item in the logbook (also I think what the tooltip is with ItemStats on?
            LanguageAPI.Add("SUBZEROPERFORATOR_LORE", "Subject: Subzero Perforator\nTechnician: Dema \"Dembones\" Brown\nTable Spec: FZ - 4\nNotes:\n\n> Using FZ - 4 due to temperatures near absolute 0.\n> Removing frozen enamel and placing aside for substance analysis. Its frozen solid, but under the icy surface its swimming.\n> Upon structural investigation, found cavities and internal bubbles.\n> Increase lab temperatures by 20 degrees.\n> Freezing water seems to be flowing underneath the icy veins - ice being supplied to the tooth?\n> Put on a second coat and gloves, shivering so much I can barely hold my tools.\n> Water from severed veins freezes upon contact with the air.\n> Grabbed a cup of hot cocoa from another room, froze solid when placed near the perforator.\n> Timestamping for bonfire break.");
        }
        public static TeamIndex? teamIndex = new TeamIndex?(TeamIndex.Player);
        private void Hooks()
        {
            //Spawning the Ice Explosion
            On.RoR2.GlobalEventManager.OnCharacterDeath += (On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport) =>
            {
                if (damageReport?.attackerBody && damageReport?.attackerMaster)
                {
                    CharacterBody character = damageReport.attackerBody;
                    if (character.inventory.GetItemCount(itemDef) > 0 && Util.CheckRoll(10f, damageReport.attackerMaster))
                    {
                        Vector3 corePosition = Util.GetCorePosition(damageReport.victim.gameObject);
                        GameObject gameObject2 = Object.Instantiate(Resources.Load<GameObject>("Prefabs/NetworkedObjects/GenericDelayBlast"), corePosition, Quaternion.identity);
                        float num = 10f + damageReport.victimBody.radius + (character.inventory.GetItemCount(itemDef) * 3);
                        float damageCoefficient = 3f * character.inventory.GetItemCount(itemDef);
                        float baseDamage = damageReport.attackerBody.damage * damageCoefficient;
                        gameObject2.transform.localScale = new Vector3(num, num, num);
                        DelayBlast component = gameObject2.GetComponent<DelayBlast>();
                        component.position = corePosition;
                        component.baseDamage = baseDamage;
                        component.baseForce = 2300f;
                        component.attacker = damageReport.attackerBody.gameObject;
                        component.radius = num;
                        component.crit = Util.CheckRoll(damageReport.victimBody.crit, damageReport.victimMaster);
                        component.procCoefficient = 1f;
                        component.maxTimer = 0f;
                        component.falloffModel = BlastAttack.FalloffModel.None;
                        component.explosionEffect = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/AffixWhiteExplosion");
                        component.delayEffect = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/AffixWhiteExplosion");
                        component.damageType = DamageType.Freeze2s;
                        gameObject2.GetComponent<TeamFilter>().teamIndex = TeamComponent.GetObjectTeam(component.attacker);
                    }
                }
                orig(self, damageReport);
            };
        }
    }
}*/