using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
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
    public class AddressablePrefab : SimpleAddress
    {
        public static readonly Dictionary<string, GameObject> PrefabCache = new Dictionary<string, GameObject>();

        public bool replaceSelf;
        public UnityEvent OnInstantiated;

        [NonSerialized]
        public GameObject instance;
        private AsyncOperationHandle<GameObject> prefabLoadOperation;
        void Update()
        {
            if (instance && lastAddress == Address) return;
            if (string.IsNullOrEmpty(Address)) return;

            lastAddress = Address;
            if (PrefabCache.ContainsKey(Address) && PrefabCache[Address])
            {
                CreateInstance();
            }
            else if (!prefabLoadOperation.IsValid() || prefabLoadOperation.Status != AsyncOperationStatus.None)
            {
                prefabLoadOperation = Addressables.LoadAssetAsync<GameObject>(Address);
                prefabLoadOperation.Completed += OnCompleted;
                prefabLoadOperation.Destroyed += (AsyncOperationHandle _) => Debug.Log("Operation has been destroyed");
            }
        }
        private void OnCompleted(AsyncOperationHandle<GameObject> aOp)
        {
            if (aOp.Status == AsyncOperationStatus.Succeeded && aOp.Result)
            {
                PrefabCache[Address] = aOp.Result;
            }
        }

        private void CreateInstance()
        {
            DestroyChildren(transform);
            instance = Instantiate(PrefabCache[Address]);
            instance.hideFlags = HideFlags.DontSave;
            instance.transform.position = transform.position;
            instance.transform.rotation = transform.rotation;
            instance.transform.localScale = transform.localScale;
            if (Application.isPlaying && replaceSelf)
            {
                Destroy(gameObject);
            }
            else
                instance.transform.parent = transform;

            SetRecursiveFlags(instance.transform);
            OnInstantiated?.Invoke();
        }


        static void SetRecursiveFlags(Transform transform)
        {
            transform.gameObject.hideFlags = HideFlags.DontSave;
            for (int i = 0; i < transform.childCount; i++)
                SetRecursiveFlags(transform.GetChild(i));
        }
        static void DestroyChildren(Transform transform)
        {
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }

        private void OnDisable()
        {
            DestroyChildren(transform);
            lastAddress = null;
        }
    }
}