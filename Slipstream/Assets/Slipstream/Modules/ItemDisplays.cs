using Moonstorm;
using RoR2.ContentManagement;
using UnityEngine;

namespace Slipstream.Modules
{
    public class ItemDisplays : ItemDisplayModuleBase
    {
        //I have no fucking clue how this works yet hold on. Just skip past this.
        
        public static ItemDisplays Instance { get; set; }
        public override AssetBundle MainBundle => SlipAssets.Instance.MainAssetBundle;
        //public override SerializableContentPack ContentPack { get; set; } = SlipContent.Instance.SerializableContentPack;

        public override void Initialize()
        {
            Instance = this;
            base.Initialize();
            AddNamedIDRSFromMainBundle();
            AddItemDisplayDictionariesFromMainBundle();
        }

        //public override AssetBundle MainBundle => null;
    }

}
