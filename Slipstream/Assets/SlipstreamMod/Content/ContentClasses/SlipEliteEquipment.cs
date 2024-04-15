using MSU;
using RoR2;
using RoR2.ContentManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Slipstream
{
    public abstract class SlipEliteEquipment : IEliteContentPiece
    {
        public abstract List<EliteDef> EliteDefs { get; }
        public abstract NullableRef<GameObject> ItemDisplayPrefab { get; }
        EquipmentDef IContentPiece<EquipmentDef>.Asset => EquipmentDef;
        public abstract EquipmentDef EquipmentDef { get; }

        public abstract bool Execute(EquipmentSlot slot);
        public abstract void OnEquipmentObtained(CharacterBody body);
        public abstract void OnEquipmentLost(CharacterBody body);
        public abstract IEnumerator LoadContentAsync();
        public abstract bool IsAvailable(ContentPack contentPack);
        public abstract void Initialize();
    }
}