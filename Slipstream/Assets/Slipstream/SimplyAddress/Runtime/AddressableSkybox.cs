using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
/**
 * MIT License
 * 
 * Copyright (c) 2022 PassivePicasso
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

namespace PassivePicasso.SimplyAddress
{
    [ExecuteAlways]
    public class AddressableSkybox : SimpleAddress
    {
        [NonSerialized]
        public Material material;
        // Start is called before the first frame update
        void Update()
        {
            if (material && material != RenderSettings.skybox)
            {
                material.hideFlags = HideFlags.HideAndDontSave;
                RenderSettings.skybox = material;
                return;
            }
            if (material && lastAddress == Address) return;
            if (string.IsNullOrEmpty(Address)) return;

            lastAddress = Address;
            var materialOp = Addressables.LoadAssetAsync<Material>(Address);
            materialOp.Completed += MaterialOp_Completed;
        }


        void MaterialOp_Completed(AsyncOperationHandle<Material> obj)
        {
            material = obj.Result;
        }
    }
}