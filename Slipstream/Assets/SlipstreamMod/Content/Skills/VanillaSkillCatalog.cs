using Moonstorm;
using R2API.ScriptableObjects;
using RoR2.ContentManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RoR2.Skills;
using RoR2;
using UnityEngine.AddressableAssets;
using HG;

namespace Slipstream.Modules
{
    public class VanillaSkillCatalog
    {
        public void Initialize()
        {
            AddSkillToFamily("Marauder", "RoR2/Base/Bandit2/Bandit2BodySpecialFamily.asset");
        }

        public static void AddSkillToFamily(string skillString, string key, string unlockString = null)
        {
            AddSkillToFamily(skillString, new string[]{ key }, unlockString);
        }

        public static void AddSkillToFamily(string skillString, string[] key, string unlockString = null)
        {
            for (int i = 0; i < key.Length; i++)
            {
                SkillDef skill = SlipAssets.Instance.MainAssetBundle.LoadAsset<SkillDef>(skillString);
                SkillFamily skillFamily = Addressables.LoadAssetAsync<SkillFamily>(key[i]).WaitForCompletion();
                UnlockableDef unlock = null;
                if (unlockString != null)
                    unlock = SlipAssets.Instance.MainAssetBundle.LoadAsset<UnlockableDef>(unlockString);
                if (skillFamily == null)
                {
                    SlipLogger.LogE($"Skill Family doesn't exist when adding " + skill.skillNameToken);
                    break;
                }
                SkillFamily.Variant variant = new SkillFamily.Variant
                {
                    skillDef = skill,
                    unlockableDef = unlock,
                    viewableNode = new ViewablesCatalog.Node(skill.skillName, false, null)
                };
                ArrayUtils.ArrayAppend<SkillFamily.Variant>(ref skillFamily.variants, variant);
            }
        }
    }
}