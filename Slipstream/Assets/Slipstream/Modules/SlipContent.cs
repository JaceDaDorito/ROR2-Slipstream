using RoR2.ContentManagement;
using System.Collections;

namespace Slipstream
{
    internal class SlipContent : IContentPackProvider
    {
        public static SerializableContentPack serializableContentPack;
        internal ContentPack contentPack;
        public string identifier => SlipMain.GUID;

        public void Init()
        {
            contentPack = serializableContentPack.CreateContentPack();
            contentPack.identifier = identifier;
            ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;
        }

        private void ContentManager_collectContentPackProviders(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(this);
        }

        public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(contentPack, args.output);
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }
    }
}
