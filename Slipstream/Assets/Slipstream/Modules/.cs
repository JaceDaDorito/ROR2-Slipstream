using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;

//This file acts as a basic skeleton for other item files, copy this file and then paste it in the correct item directory, and then make changes to that file 

namespace Slipstream.Items
{
    public class SnakeEyes
    {
        public static ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();

        public void Init() //This runs whatever methods you need to for the item, such as language stuff, effects, setting up the item, etc. Init is ran in the Awake() method in the Main.cs file and every item you add you have to run Init() inside of Awake() in the Main.cs file or else it won't work.
        {
            //SnakeEyesItem();
            AddLanguageTokens();
            Hooks();
        }

        /*private void SnakeEyesItem()
        {
            itemDef.name = "SnakeEyes"; //Internal name of the item
            itemDef.tier = ItemTier.Tier1; //Tier of the item, Tier1 = White. Tier2 = Green, Tier3 = Red. Lunar = Blue. Boss = Yellow
            itemDef.pickupModelPrefab = SlipstreamPlugin.AssetBundle.LoadAsset<GameObject>("Assets/Import/Cube/cube.prefab"); //3D Model of the item in the world/Logbook
            itemDef.pickupIconSprite = SlipstreamPlugin.AssetBundle.LoadAsset<Sprite>("Assets/Import/Cube/cube_icon.png"); //Sprite of the item  when in your inventory/Logbook
            itemDef.nameToken = "SNAKEEYES_NAME"; //Name of the item ingame
            itemDef.pickupToken = "SNAKEEYES_PICKUP"; //Text when you hover over the item in your inventoy or the tooltip when you pick it up
            itemDef.descriptionToken = "SNAKEEYES_DESC"; //Description of the item in the logbook (also I think what the tooltip is with ItemStats on?
            itemDef.loreToken = "SNAKEEYES_LORE"; //Lore of the item in the logbook
            itemDef.tags = new ItemTag[] { ItemTag.Damage }; //Tags for the item, for stuff such as classifying it for Utility/Damage/Healing chests, if it's blacklisted from Mithrix, if it's scrap, if it's sprint related, etc.

            //TODO: MAKE THIS SHIT ACTUALLY WORK
            var itemDisplayRules = new ItemDisplayRule[1];
            itemDisplayRules[0].followerPrefab = SlipstreamPlugin.AssetBundle.LoadAsset<GameObject>("Assets/Import/snake/snake.prefab");
            itemDisplayRules[0].childName = "Chest";
            itemDisplayRules[0].localScale = new Vector3(2f, 2f, 2f);

            SlipstreamPlugin.ModItemDefs.Add(itemDef); //Adds this item to the list of itemDefs in Main.cs, which then gets added to the ContentPack, which then gets added to the game's content
        }*/

        private void AddLanguageTokens()
        {
            //Name of the item ingame
            LanguageAPI.Add("SNAKEEYES_NAME", "Snake Eyes");

            //Text when you hover over the item in your inventoy or the tooltip when you pick it up
            LanguageAPI.Add("SNAKEEYES_PICKUP", "Failing a shrine increases critical strike chance"); 

            //Description of the item in the logbook (also I think what the tooltip is with ItemStats on?)
            LanguageAPI.Add("SNAKEEYES_DESC", "<style=cIsDamage>6%</style> <style=cStack>(+6% per stack)</style> increased chance to '<style=cIsDamage>Critically Strike</style>' upon failing a Shrine of Chance. Removes all stacks upon winning a Shrine of Chance.");
            
            //Lore of the item in the logbook. \n inserts a new line.  
            LanguageAPI.Add("SNAKEEYES_LORE", "Tracking Number: 723***********\n\nShipping Method: Standard\n\nOrder Details: You dirty---------er. You KNEW I had to win to pay off my debts. Are you in with the casinos? Of course you are; a snake like you would. A dice that's loaded for SNAKE EYES. CUTE MOVE, ---------er.\nI'm comin' for you, ----."); 
        }

        private void Hooks()
        {
            //This is where effects for the item would be written, if you want a full example, check out /Items/RottingPerforator 
        }

    }
}