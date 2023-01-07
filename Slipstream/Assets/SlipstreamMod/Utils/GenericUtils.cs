using KinematicCharacterController;
using Moonstorm;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using Slipstream;
using UnityEngine.UI;
using AddressablesHelper;
using UnityEngine.AddressableAssets;
using static RoR2.CharacterModel;
using R2API;

namespace Slipstream
{
    public static class GenericUtils
    {
        public static void RemoveStacksOfBuff(CharacterBody body, BuffDef buffDef, int count)
        {
            for(int i = 0; i < count; i++)
            {
                body.RemoveBuff(buffDef);
            }
        }
    }
}
