using Moonstorm;
using R2API.ScriptableObjects;
using RoR2.ContentManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Slipstream.Modules
{
    public class VanillaSkillCatalog
    {
        public R2APISerializableContentPack SerializableContentPack => SlipContent.Instance.SerializableContentPack;

        public void Initialize()
        {
            SlipLogger.LogI($"Initializing Skills...");
        }
    }
}