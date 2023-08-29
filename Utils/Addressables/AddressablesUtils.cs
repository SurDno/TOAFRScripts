using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TOAFL.Utils.Addressables
{
    public static class AddressablesUtils
    {
         private static string CoreDisabledObjectName => $"[DontChange]{nameof(AddressablesUtils)}-DisabledObjectHelper";

        private static Transform HiddenCoreObject => _hiddenCoreObject != null 
            ? _hiddenCoreObject 
            : _hiddenCoreObject = GetDisabledCoreObject() ;
        
        private static Transform _hiddenCoreObject = null;
        
        /// <summary>
        /// Will instantiate an object disabled preventing it from calling Awake/OnEnable.
        /// </summary>
        public static async UniTask<GameObject> InstantiateDisabledAsync(object key, Transform parent = null, bool worldPositionStays = false)
        {
            return await InstantiateInternal(key, Vector3.zero, Quaternion.identity, parent, worldPositionStays);
        }

        /// <summary>
        /// Will instantiate an object disabled preventing it from calling Awake/OnEnable.
        /// </summary>
        public static async UniTask<GameObject> InstantiateDisabledAsync(object key, Vector3 position, Quaternion rotation,
            Transform parent = null, bool worldPositionStays = false)
        {
            return await InstantiateInternal(key, position, rotation, parent, worldPositionStays);
        }

        /// <summary>
        /// Will instantiate an object disabled preventing it from calling Awake/OnEnable.
        /// </summary>
        public static async UniTask<T> InstantiateDisabledAsync<T>(object key, Transform parent = null, bool worldPositionStays = false)
            where T : Component
        {
            var gameObject = await InstantiateInternal(key, Vector3.zero, Quaternion.identity, parent, worldPositionStays);
            var component = gameObject.GetComponent<T>();
            
            return component;
        }

        /// <summary>
        /// Will instantiate an object disabled preventing it from calling Awake/OnEnable.
        /// </summary>
        public static async UniTask<T> InstantiateDisabledAsync<T>(object key, Vector3 position, Quaternion rotation,
            Transform parent = null, bool worldPositionStays = false) where T : Component
        {
            var gameObject = await InstantiateInternal(key, position, rotation, parent, worldPositionStays);
            var component = gameObject.GetComponent<T>();
            
            return component;
        }


        private static async UniTask<GameObject> InstantiateInternal(object key, Vector3 position, Quaternion rotation, Transform parent,
            bool worldPositionStays)
        {
            if (!GetActiveState(await UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<GameObject>(key)))
            {
                return await UnityEngine.AddressableAssets.Addressables.InstantiateAsync(key, position, rotation, parent);
            }

            var instance = await UnityEngine.AddressableAssets.Addressables.InstantiateAsync(key, position, rotation, HiddenCoreObject);

            SetActiveState(instance, false);
            SetParent(instance, parent, worldPositionStays);

            return instance;
        }

        private static bool GetActiveState<T>(T @object) where T : UnityEngine.Object
        {
            switch (@object)
            {
                case GameObject gameObject:
                {
                    return gameObject.activeSelf;
                }
                case Component component:
                {
                    return component.gameObject.activeSelf;
                }
                default:
                {
                    return false;
                }
            }
        }

        private static void SetActiveState<T>(T @object, bool state) where T : UnityEngine.Object
        {
            switch (@object)
            {
                case GameObject gameObject:
                {
                    gameObject.SetActive(state);

                    break;
                }
                case Component component:
                {
                    component.gameObject.SetActive(state);

                    break;
                }
            }
        }

        private static void SetParent<T>(T @object, Transform parent, bool worldPositionStays) where T : UnityEngine.Object
        {
            switch (@object)
            {
                case GameObject gameObject:
                {
                    gameObject.transform.SetParent(parent, worldPositionStays);

                    break;
                }
                case Component component:
                {
                    component.transform.SetParent(parent, worldPositionStays);

                    break;
                }
            }
        }
        
        private static Transform GetDisabledCoreObject()
        {
            _hiddenCoreObject = new GameObject(CoreDisabledObjectName)
                    { hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector }
                .transform;

            _hiddenCoreObject.gameObject.SetActive(false);

            return _hiddenCoreObject;
        }
    }
}