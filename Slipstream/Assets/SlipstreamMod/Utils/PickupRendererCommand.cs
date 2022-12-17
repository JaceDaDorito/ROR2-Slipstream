using RoR2;
using UnityEngine;
using R2API.Utils;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Slipstream
{
    //Thanks GrooveSalad for lending this code to me. This code is taken from Starstorm 2.
    //this is temporary code until someone makes a tool to render shit in game
    public class PickupRendererCommand
    {
        static PickupDef selectedPickupDef;
        //CommandHelper
        [ConCommand(commandName = "render_pickup", flags = ConVarFlags.None, helpText = "Render Pickup Icon")]
        public static void CCRenderItem(ConCommandArgs args)
        {
            SlipLogger.LogM($"render pickup!");
            string idealPickupName = args.TryGetArgString(0);
            
            if (string.IsNullOrEmpty(idealPickupName)){
                SlipLogger.LogD($"Please input a valid pickup name");
                return;
            }
            foreach (PickupDef pickupDef in PickupCatalog.allPickups)
            {
                string currentPickupDefName = pickupDef.internalName.Substring(pickupDef.internalName.IndexOf(".") + 1).ToLower();
                if( currentPickupDefName.Contains(idealPickupName.ToLower()) && !pickupDef.internalName.StartsWith("Artifact"))
                {
                    selectedPickupDef = pickupDef;
                    Addressables.LoadSceneAsync("722873b571c73734c8572658dbb8f0db").Completed += SlipstreamRenderItemIcons_Completed;
                    
                    return;
                }
            }
            SlipLogger.LogD("Pickup with name " + idealPickupName + " not found.");
        }

        private static void SlipstreamRenderItemIcons_Completed(AsyncOperationHandle<SceneInstance> sceneInstance)
        {
            Scene scene = sceneInstance.Result.Scene;
            GameObject[] rootobjects = scene.GetRootGameObjects();
            for(int i = 0; i< rootobjects.Length; i++)
            {
                if(rootobjects[i].name == "ITEM GOES HERE (can offset from here)")
                {
                    GameObject gameObject = rootobjects[i];
                    foreach (ItemDisplay itemDisplay in gameObject.GetComponentsInChildren<ItemDisplay>())
                    {
                        itemDisplay.gameObject.SetActive(false);
                    }
                    GameObject display = UnityEngine.Object.Instantiate(selectedPickupDef.displayPrefab);
                    display.transform.SetParent(gameObject.transform);
                    display.transform.localPosition = Vector3.zero;
                }
            }
        }

        public void Init()
        {
            CommandHelper.AddToConsoleWhenReady();
        }
    }
}
