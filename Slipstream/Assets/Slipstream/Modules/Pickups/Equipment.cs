using Moonstorm;
using R2API.ScriptableObjects;
using RoR2;
using System.Collections.Generic;
using System.Linq;

namespace Slipstream.Modules
{
    public class Equipments : EquipmentModuleBase
    {
        //Loads all Slipstream pickups like Equipment and Items
        public static Equipments Instance { get; set; }
        public static EquipmentDef[] LoadedSlipItems { get => SlipContent.Instance.SerializableContentPack.equipmentDefs; }

        public override R2APISerializableContentPack SerializableContentPack => SlipContent.Instance.SerializableContentPack;

        public override void Initialize()
        {
            Instance = this;
            base.Initialize();
            SlipLogger.LogD($"Initializing Slipstream Equipment");
            GetEquipmentBases();
        }
        protected override IEnumerable<EquipmentBase> GetEquipmentBases()
        {
            base.GetEquipmentBases()
                .Where(eqp => SlipMain.config.Bind<bool>(eqp.EquipmentDef.name, "Enable Equipment", true, "Wether or not to enable this equipment").Value)
                .ToList()
                .ForEach(eqp => AddEquipment(eqp));
            return null;
        }

        protected override IEnumerable<EliteEquipmentBase> GetEliteEquipmentBases()
        {
            base.GetEliteEquipmentBases()
                .ToList()
                .ForEach(eeqp => AddEliteEquipment(eeqp));
            return null;
        }
    }
}
