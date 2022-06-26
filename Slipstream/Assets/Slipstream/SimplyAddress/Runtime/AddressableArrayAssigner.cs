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
    public abstract class AddressableArrayAssigner<ComponentType, AssetType> : SimpleAddress where AssetType : UnityEngine.Object where ComponentType : Component
    {
        public ComponentType[] TargetComponents;
        [NonSerialized]
        public AssetType AssetInstance;

        void Update()
        {
            if (lastAddress == Address) return;

            lastAddress = Address;
            if (string.IsNullOrEmpty(Address)) return;
            Load();
        }

        private void Load()
        {
            var loadOperation = Addressables.LoadAssetAsync<AssetType>(Address);
            loadOperation.Completed += OnLoaded;
        }

        private void OnEnable()
        {
            if (!AssetInstance)
                Load();
            else
                AssignInternal();
        }

        private void OnDisable()
        {
            UnassignInternal();
        }

        void OnLoaded(AsyncOperationHandle<AssetType> obj)
        {
            if (!obj.Result) return;
            obj.Result.hideFlags = HideFlags.NotEditable | HideFlags.HideAndDontSave;
            AssetInstance = Instantiate(obj.Result);
            AssetInstance.hideFlags = HideFlags.NotEditable | HideFlags.HideAndDontSave;
            AssetInstance.name = AssetInstance.name.Replace("(Clone)", "(WeakReference)");
            AssignInternal();
        }

        private void AssignInternal()
        {
            if (AssetInstance)
                foreach (var component in TargetComponents)
                    Assign(component, AssetInstance);
        }
        private void UnassignInternal()
        {
            if (AssetInstance)
                foreach (var component in TargetComponents)
                    Unassign(component, AssetInstance);
        }

        protected abstract void Assign(ComponentType component, AssetType asset);
        protected abstract void Unassign(ComponentType component, AssetType asset);
    }
}