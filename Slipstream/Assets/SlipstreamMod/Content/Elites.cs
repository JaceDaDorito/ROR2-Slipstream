//using Slipstream.Elites;
using Moonstorm;
using R2API.ScriptableObjects;
using RoR2;
using RoR2.ContentManagement;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace Slipstream.Modules
{
    public sealed class Elites : EliteModuleBase
    {
        public static Elites Instance { get; set; }
        public static MSEliteDef[] LoadedSlipElites { get => SlipContent.Instance.SerializableContentPack.eliteDefs as MSEliteDef[]; }
        public override R2APISerializableContentPack SerializableContentPack => SlipContent.Instance.SerializableContentPack;
        public override AssetBundle AssetBundle => SlipAssets.Instance.GetAssetBundle(SlipBundle.Elites);


        public override void Initialize()
        {
            Instance = this;
            base.Initialize();
            SlipLogger.LogI($"Initializing Elites...");
            GetInitializedEliteEquipmentBases();
        }

        protected override IEnumerable<EliteEquipmentBase> GetInitializedEliteEquipmentBases()
        {
            base.GetInitializedEliteEquipmentBases()
                .Where(elite => SlipMain.config.Bind<bool>("Slipstream Elites", elite.GetType().Name, true, "Enable/disable this Elite Type.").Value)
                .ToList()
                .ForEach(elite => AddElite(elite));
            return null;
        }

        private void LateEliteSetup(ReadOnlyCollection<MSEliteDef> eliteCollection)
        {
            //Setup goes here

            /*for(int i = 0; i < LoadedSlipElites.Length; i++)
            {
                Slipstream.SlipstreamEliteRamps.SlipstreamEliteRamp item = new SlipstreamEliteRamps.SlipstreamEliteRamp();
                item.eliteDef = LoadedSlipElites[i];
                item.rampTexture = LoadedSlipElites[i].eliteRamp;
                SlipstreamEliteRamps.eliteRamps.Add(item);
            }*/
        }

    }
}
