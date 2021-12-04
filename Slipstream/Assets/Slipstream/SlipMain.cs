using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.ContentManagement;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Path = System.IO.Path;
using static R2API.RecalculateStatsAPI;

namespace Slipstream
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.TeamMoonstorm.MoonstormSharedUtils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ModGuid, ModName, ModVer)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [R2APISubmoduleDependency(nameof(ItemAPI), nameof(ItemDropAPI), nameof(LanguageAPI), nameof(ResourcesAPI), nameof(PrefabAPI), nameof(BuffAPI), nameof(LoadoutAPI), nameof(ProjectileAPI), nameof(RecalculateStatsAPI))]
    public class SlipMain : BaseUnityPlugin
    {
        private const string ModVer = "0.0.1";
        private const string ModName = "Slipstream";
        public const string ModGuid = "com.TeamSlipstream.Slipstream";
        public static string MODPREFIX = "@Slipstream:";
        public string identifier => "com.TeamSlipstream.Slipstream";

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}