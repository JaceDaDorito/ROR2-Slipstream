﻿using Moonstorm;
using RoR2;
using RoR2.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Slipstream.Modules
{
    public class Pickups : PickupModuleBase
    {
        public static Pickups Instance { get; set; }
        public static ItemDef[] LoadedSlipItems { get => SlipContent.serializableContentPack.itemDefs; }
        public static EquipmentDef[] LoadedSlipEquipsments { get => SlipContent.serializableContentPack.equipmentDefs; }
        public override SerializableContentPack ContentPack { get; set; } = SlipContent.serializableContentPack;

        public override void Init()
        {
            Instance = this;
            base.Init();
            if (SlipConfig.EnableItems.Value)
            {
                InitializeItems();
            }
            if (SlipConfig.EnableEquipments.Value) 
            {
                InitializeEquipments();
                InitializeEliteEquipments();
            }
        }
        public override IEnumerable<ItemBase> InitializeItems()
        {
            base.InitializeItems().Where(item => SlipMain.config.Bind<bool>(item.ItemDef.name, "Enable Item", true, "wether or not to enable this item.").Value)
                .ToList()
                .ForEach(item => AddItem(item, ContentPack));
            return null;
        }
        public override IEnumerable<EquipmentBase> InitializeEquipments()
        {
            base.InitializeEquipments().Where(equip => SlipMain.config.Bind<bool>(equip.EquipmentDef.name, "Enable Equipment", true, "Wether or not to enable this equipment.").Value)
                .ToList()
                .ForEach(equip => AddEquipment(equip, ContentPack));
            return null;
        }
        public override IEnumerable<EliteEquipmentBase> InitializeEliteEquipments()
        {
            base.InitializeEliteEquipments().ToList().ForEach(equip => AddEliteEquipment(equip));
            return null;
        }
    }
}