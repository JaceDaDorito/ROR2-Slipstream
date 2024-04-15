using RoR2.ContentManagement;
using R2API.ScriptableObjects;
using R2API.ContentManagement;
using System;
using System.Linq;
using RoR2;
using UnityEngine;
using System.Collections;
using MSU;
using RoR2.ExpansionManagement;

namespace Slipstream
{
    public class SlipContent : IContentPackProvider
    {
        public string identifier => SlipMain.GUID;
        public static ReadOnlyContentPack ReadOnlyContentPack => new ReadOnlyContentPack(SlipContentPack);
        internal static ContentPack SlipContentPack { get; } = new ContentPack();

        internal static ParallelCoroutineHelper _parallelPreLoadDispatchers = new ParallelCoroutineHelper();
        internal static Func<IEnumerator>[] _loadDispatchers;
        internal static ParallelCoroutineHelper _parallelPostLoadDispatchers = new ParallelCoroutineHelper();

        private static Action[] _fieldAssignDispatchers;

        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            var enumerator = SlipAssets.Initialize();
            while (enumerator.MoveNext()) yield return null;

            _parallelPreLoadDispatchers.Start();
            while (!_parallelPreLoadDispatchers.IsDone()) yield return null;

            for (int i = 0; i < _loadDispatchers.Length; i++)
            {
                args.ReportProgress(Util.Remap(i + 1, 0f, _loadDispatchers.Length, 0.1f, 0.2f));
                enumerator = _loadDispatchers[i]();

                while (enumerator.MoveNext())
                    yield return null;
            }

            _parallelPostLoadDispatchers.Start();
            while (!_parallelPostLoadDispatchers.IsDone()) yield return null;

            for (int i = 0; i < _fieldAssignDispatchers.Length; i++)
            {
                args.ReportProgress(Util.Remap(i + 1, 0f, _loadDispatchers.Length, 0.1f, 0.2f));
                _fieldAssignDispatchers[i]();
                yield return null;
            }
        }

        public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(SlipContentPack, args.output);
            yield break;
        }

        private void AddSelf(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(this);
        }

        private IEnumerator AddExpansionDef()
        {
            SlipAssetRequest<ExpansionDef> expansionDefRequest = SlipAssets.LoadAssetAsync<ExpansionDef>("SlipExpansionDef", SlipBundle.Main);

            expansionDefRequest.StartLoad();
            while (!expansionDefRequest.IsComplete)
                yield return null;

            SlipContentPack.expansionDefs.AddSingle(expansionDefRequest.Asset);
        }

        internal SlipContent()
        {
            ContentManager.collectContentPackProviders += AddSelf;

            SlipAssets.AssetsAvailability.CallWhenAvailable(() =>
            {
                _parallelPreLoadDispatchers.Add(SlipConfig.RegisterToModSettingsManager);
                _parallelPreLoadDispatchers.Add(AddExpansionDef);
            });

            SlipMain main = SlipMain.instance;
            _loadDispatchers = new Func<IEnumerator>[]
            {
                () =>
                {
                    EquipmentModule.AddProvider(main, ContentUtil.CreateContentPieceProvider<EquipmentDef>(main, SlipContentPack));
                    return EquipmentModule.InitialzeEquipments(main);
                },
                () =>
                {
                    ItemModule.AddProvider(main, ContentUtil.CreateContentPieceProvider<ItemDef>(main, SlipContentPack));
                    return ItemModule.InitializeItems(main);
                },
                () =>
                {
                    CharacterModule.AddProvider(main, ContentUtil.CreateGameObjectContentPieceProvider<CharacterBody>(main, SlipContentPack));
                    return CharacterModule.InitializeCharacters(main);
                }
            };

            _fieldAssignDispatchers = new Action[]
            {
                () =>
                {
                    ContentUtil.PopulateTypeFields(typeof(Buffs), SlipContentPack.buffDefs);
                },
                () =>
                {
                    ContentUtil.PopulateTypeFields(typeof(Elites), SlipContentPack.eliteDefs);
                },
                () =>
                {
                    ContentUtil.PopulateTypeFields(typeof(Equipments), SlipContentPack.equipmentDefs);
                },
                () =>
                {
                    ContentUtil.PopulateTypeFields(typeof(Items), SlipContentPack.itemDefs);
                },
                () =>
                {
                    ContentUtil.PopulateTypeFields(typeof(Scenes), SlipContentPack.sceneDefs);
                }
            };
        }

        //Handles Content implementation, add Definitions to items here accordingly.
        public static class Buffs
        {
            //Buffs go here
            //public static BuffDef BedeviledMark;
            public static BuffDef AffixSandswept;
            public static BuffDef Grainy; //Sandswept onhit debuff
            public static BuffDef Chipped;

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
            //public static BodyDef
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
            public static SceneDef aridexpanse;
        }
    }
}
   