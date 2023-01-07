using RoR2.ContentManagement;
using R2API.ScriptableObjects;
using R2API.ContentManagement;
using Moonstorm.Loaders;
using System;
using System.Collections.Generic;
using Slipstream.Modules;
using System.Linq;
using RoR2;
using UnityEngine;
using Moonstorm;


namespace Slipstream
{
    public class SlipContent : ContentLoader<SlipContent>
    {
        //Handles Content implementation, add Definitions to items here accordingly.
        public static class Buffs
        {
            //Buffs go here
            //public static BuffDef BedeviledMark;
            public static BuffDef AffixSandswept;
            public static BuffDef Grainy; //Sandswept onhit debuff

            public static BuffDef BrineBuff;
            public static BuffDef PepperSpeed;
            public static BuffDef SoulRoot;
            public static BuffDef ChungusBuff;
        }

        public static class Characters
        {
            #region Survivors
            #endregion

            #region Enemies
            #endregion
        }
        public static class Elites
        {
            //Elites go here
            public static EliteDef Sandswept;
        }
        public static class Equipments
        {
            //Equipments go here
            #region Orange equips
            public static EquipmentDef AffixSandswept;
            public static EquipmentDef Incubator;
            public static EquipmentDef SoulSucker;
            #endregion

            #region Lunar equips
            #endregion
        }
        
        public static class Items
        {
            //Items go here
            #region White items
            public static ItemDef FriendshipBracelet;
            public static ItemDef RustyGenerator;
            #endregion

            #region Green items
            public static ItemDef PepperSpray;
            public static ItemDef GlassEye;
            //public static ItemDef JaceHat;
            #endregion

            #region Red items
            public static ItemDef ChargedFungus;
            #endregion

            #region Yellow items
            #endregion

            #region Void Items
            public static ItemDef BrineSwarm;
            #endregion

            #region Lunar items
            public static ItemDef Coalition;
            #endregion

            #region Untiered items
            //very rarely used
            public static ItemDef BlackHealth;
            #endregion

        }

        public static class Scenes
        {
            public static SceneDef TestStage;
            public static SceneDef aridexpanse;
        }
        public override string identifier => SlipMain.GUID;

        public override R2APISerializableContentPack SerializableContentPack { get; protected set; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<R2APISerializableContentPack>("ContentPack");
        public override Action[] LoadDispatchers { get; protected set; }
        public override Action[] PopulateFieldsDispatchers { get; protected set; }

        public override void Init()
        {
            base.Init();
            LoadDispatchers = new Action[]
            {
                #region delegates
                delegate
                {
                    new Slipstream.Buffs.Buffs().Initialize();
                },
                delegate
                {
                    new Modules.Projectiles().Initialize();
                },
                
                delegate
                {
                    new Modules.Equipments().Initialize();
                },
                delegate
                {
                    new Modules.Items().Initialize();
                },
                delegate
                {
                    new Modules.Elites().Initialize();
                },
                delegate
                {
                    new Modules.Scenes().Initialize();
                },
                delegate
                {
                    new Modules.Unlocks().Initialize();
                },
                delegate
                {
                    new VanillaSkillCatalog().Initialize();
                },
                /*delegate
                {
                    new Modules.VanillaSkillCatalog().Initialize();
                },*/
                /*delegate
                {
                    new ItemDisplays().Initialize();
                },*/
                delegate
                {
                    SerializableContentPack.entityStateTypes = typeof(SlipContent).Assembly.GetTypes()
                        .Where(type => typeof(EntityStates.EntityState).IsAssignableFrom(type))
                        .Select(type => new EntityStates.SerializableEntityStateType(type))
                        .ToArray();
                },
                delegate
                {
                   SerializableContentPack.effectPrefabs = SlipAssets.LoadAllAssetsOfType<GameObject>().Where(go => go.GetComponent<EffectComponent>()).ToArray();
                },
                delegate
                {
                   SlipAssets.Instance.SwapMaterialShaders();
                }
                #endregion
            };

            PopulateFieldsDispatchers = new Action[]
            {
                #region delegates
                delegate
                {
                    PopulateTypeFields(typeof(Buffs), ContentPack.buffDefs);
                },
                delegate
                {
                    PopulateTypeFields(typeof(Elites), ContentPack.eliteDefs);
                },
                delegate
                {
                    PopulateTypeFields(typeof(Equipments), ContentPack.equipmentDefs);
                },
                delegate
                {
                    PopulateTypeFields(typeof(Items), ContentPack.itemDefs);
                },
                delegate
                {
                    PopulateTypeFields(typeof(Scenes), ContentPack.sceneDefs);
                }
                #endregion
            };
        }
    }
}
   