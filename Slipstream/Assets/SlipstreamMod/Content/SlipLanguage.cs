using RoR2;
using Moonstorm.Loaders;

namespace Slipstream.Modules
{
    public class SlipLanguage : LanguageLoader<SlipLanguage>
    {
        //Detects and loads all of the Language files
        public override string AssemblyDir => SlipAssets.Instance.AssemblyDir;
        public override string LanguagesFolderName => "SlipLang";

        internal void Init()
        {
            SlipLogger.LogD($"Initializing Slipstream Languages");
            LoadLanguages();
        }
    }
}
