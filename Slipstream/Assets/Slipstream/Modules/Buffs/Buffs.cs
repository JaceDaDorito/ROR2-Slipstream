using Moonstorm;
using R2API.ScriptableObjects;
using RoR2;
using RoR2.ContentManagement;
using System.Collections.Generic;
using System.Linq;

namespace Slipstream.Buffs
{
    public sealed class Buffs : BuffModuleBase
    {
        //Loads all Slipstream buffs
        public static Buffs Instance { get; set; }
        public static BuffDef[] LoadedSlipBuffs { get => SlipContent.Instance.SerializableContentPack.buffDefs; }
        public override R2APISerializableContentPack SerializableContentPack => SlipContent.Instance.SerializableContentPack;

        public override void Initialize()
        {
            Instance = this;
            base.Initialize();
            SlipLogger.LogI($"Initializing Slipstream Buffs");
            GetBuffBases();
        }

        //sends contentpack buffs to be iterated and populated.
        protected override IEnumerable<BuffBase> GetBuffBases()
        {
            base.GetBuffBases()
                .ToList()
                .ForEach(buff => AddBuff(buff));
            return null;
        }
    }
}
