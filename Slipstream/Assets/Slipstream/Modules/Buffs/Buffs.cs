using Moonstorm;
using RoR2;
using RoR2.ContentManagement;
using System.Collections.Generic;
using System.Linq;

namespace Slipstream.Buffs
{
    public class Buffs : BuffModuleBase
    {
        public static Buffs Instance { get; set; }
        public static BuffDef[] LoadedSlipBuffs { get => SlipContent.Instance.SerializableContentPack.buffDefs; }
        public override SerializableContentPack ContentPack { get; set; } = SlipContent.Instance.SerializableContentPack;

        public override void Init()
        {
            Instance = this;
            base.Init();
            SlipLogger.LogI($"Initializing Slipstream Buffs");
            InitializeBuffs();
        }

        //sends contentpack buffs to be iterated and populated.
        public override IEnumerable<BuffBase> InitializeBuffs()
        {
            base.InitializeBuffs().ToList().ForEach(buff => AddBuff(buff, ContentPack));
            return null;
        }
    }
}
