using Moonstorm;
using R2API.ScriptableObjects;
using RoR2.ContentManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Slipstream.Modules
{
    public class Characters : CharacterModuleBase
    {
        public static Characters Instance { get; set; }
        public override R2APISerializableContentPack SerializableContentPack => SlipContent.Instance.SerializableContentPack;
        public override void Initialize()
        {
            Instance = this;
            base.Initialize();
            SlipLogger.LogI($"Initializing Characters and Enemies...");
            GetCharacterBases();
        }

        protected override IEnumerable<CharacterBase> GetCharacterBases()
        {
            base.GetCharacterBases()
                .ToList()
                .ForEach(Characters => AddCharacter(Characters));
            return null;
        }
    }
}