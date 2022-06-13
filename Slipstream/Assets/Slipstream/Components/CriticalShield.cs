using Moonstorm;
using RoR2;
using RoR2.Items;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using R2API;
using On.RoR2;
using On.RoR2.UI;
using TMPro;
using UnityEngine.UI;

namespace Slipstream.Components
{
    //hm
    public class CriticalShield : MonoBehaviour
    {
        
        private static Sprite lowShieldNormal;
        private static Sprite lowShieldVoid;

        private Image image;

        private bool shouldTrigger = false;
        // Start is called before the first frame update

        private void Awake()
        {
            lowShieldNormal = SlipAssets.Instance.MainAssetBundle.LoadAsset<Sprite>("texCriticalShieldIndi");
            lowShieldVoid = SlipAssets.Instance.MainAssetBundle.LoadAsset<Sprite>("texCriticalVoidShiIndi");
        }
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }
    }


}
