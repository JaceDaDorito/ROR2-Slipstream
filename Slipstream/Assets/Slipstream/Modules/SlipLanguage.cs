﻿using RoR2;
using Moonstorm.Loaders;

namespace Slipstream.Modules
{
    public class SlipLanguage : LanguageLoader<SlipLanguage>
    {
        public override string AssemblyDir => SlipAssets.Instance.AssemblyDir;
        public override string LanguagesFolderName => "Languages";

        internal void Init()
        {
            LoadLanguages();
        }
    }
}