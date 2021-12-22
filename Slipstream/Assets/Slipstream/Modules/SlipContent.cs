using RoR2.ContentManagement;
using Moonstorm.Loaders;
using System;
using Slipstream.Modules;
using System.Linq;
using RoR2;

namespace Slipstream
{
    public class SlipContent : ContentLoader<SlipContent>
    {
        //Handles Content implementation, add Definitions to items here accordingly.
        public static class Buffs
        {
            //Buffs go here
            public static BuffDef PepperSpeed;
        }

        public static class Elites
        {
            //Elites go here, don't put anything here though, I have't implmented stuff regarding elites yet. We aren't doing anything regarding elites yet too.
        }
        
        public static class Equipments
        {
            //Equipments go here
            #region Orange equips
            #endregion

            #region Lunar equips
            #endregion
        }

        public static class Items
        {
            //Items go here
            #region White items
            public static ItemDef PepperSpray;
            #endregion

            #region Green items
            public static ItemDef JaceHat;
            #endregion

            #region Red items
            #endregion

            #region Yellow items
            #endregion

            #region Lunar items
            #endregion

            #region Untiered items
            //very rarely used
            #endregion

        }
        public override string identifier => SlipMain.GUID;

        public override SerializableContentPack SerializableContentPack { get; protected set; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<SerializableContentPack>("ContentPack");
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
                    new Slipstream.Buffs.Buffs().Init();
                },
                delegate
                {
                    new Modules.Projectiles().Init();
                },
                delegate
                {
                    new Pickups().Init();
                },
                delegate
                {
                    new ItemDisplays().Init();
                },
                delegate
                {
                    typeof(SlipContent).Assembly.GetTypes()
                        .Where(type => typeof(EntityStates.EntityState).IsAssignableFrom(type))
                        .ToList()
                        .ForEach(state => HG.ArrayUtils.ArrayAppend(ref SerializableContentPack.entityStateTypes, new EntityStates.SerializableEntityStateType(state)));
                },
                delegate
                {
                    SlipAssets.Instance.LoadEffectDefs();
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
                    PopulateTypeFields(typeof(Equipments), ContentPack.equipmentDefs);
                },
                delegate
                {
                    PopulateTypeFields(typeof(Items), ContentPack.itemDefs);
                }
                #endregion
            };
        }
    }
}
   