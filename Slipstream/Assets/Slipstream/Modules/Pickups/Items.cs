using Moonstorm;
using R2API.ScriptableObjects;
using RoR2;
using System.Collections.Generic;
using System.Linq;

namespace Slipstream.Modules
{
    public class Items : ItemModuleBase
    {
        //Loads all Slipstream pickups like Equipment and Items
        public static Items Instance { get; set; }
        public static ItemDef[] LoadedSlipItems { get => SlipContent.Instance.SerializableContentPack.itemDefs; }

        public override R2APISerializableContentPack SerializableContentPack => SlipContent.Instance.SerializableContentPack;

        public override void Initialize()
        {
            Instance = this;
            base.Initialize();
            SlipLogger.LogD($"Initializing Slipstream Items");
            GetItemBases();
        }
        protected override IEnumerable<ItemBase> GetItemBases()
        {
            base.GetItemBases()
                .Where(item => SlipMain.config.Bind<bool>(item.ItemDef.name, "Enable Item", true, "Wether or not to enable this item.").Value)
                .ToList()
                .ForEach(item => AddItem(item, null));
            return null;
        }
    }
}
