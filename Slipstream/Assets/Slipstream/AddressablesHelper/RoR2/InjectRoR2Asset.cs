using UnityEngine;
using UnityEngine.AddressableAssets;

namespace AddressablesHelper
{
    [ExecuteAlways]
    public class InjectRoR2Asset : MonoBehaviour
    {
        public string Key;

        private GameObject instance;

        private void OnEnable()
        {
            Refresh();
        }

        private void OnDisable()
        {
            if (instance)
            {
                DestroyImmediate(instance);
            }
        }

        private void OnValidate()
        {
            Refresh();
        }

        private void Refresh()
        {
            if (instance)
            {
                DestroyImmediate(instance);
            }

            instance = Instantiate(Addressables.LoadAssetAsync<GameObject>(Key).WaitForCompletion(), gameObject.transform);
        }
    }
}
