using UnityEngine;

namespace TOAFL.Utils.Unity
{
    public static partial class UnityUtils
    {
        public static class Object
        {
            private static string CoreDisabledObjectName => $"[DontChange]{nameof(UnityUtils)}-{nameof(Object)}-DisabledObjectHelper";

            private static Transform HiddenCoreObject => _hiddenCoreObject != null 
                ? _hiddenCoreObject 
                : _hiddenCoreObject = GetDisabledCoreObject() ;
        
            private static Transform _hiddenCoreObject = null;
            
            /// <summary>
            /// Will instantiate an object disabled preventing it from calling Awake/OnEnable.
            /// </summary>
            public static T InstantiateDisabled<T>(T original, Transform parent = null, bool worldPositionStays = false)
                where T : UnityEngine.Object
            {
                return InstantiateInternal(original, Vector3.zero, Quaternion.identity, parent, worldPositionStays);
            }

            /// <summary>
            /// Will instantiate an object disabled preventing it from calling Awake/OnEnable.
            /// </summary>
            public static T InstantiateDisabled<T>(T original, Vector3 position, Quaternion rotation,
                Transform parent = null, bool worldPositionStays = false) where T : UnityEngine.Object
            {
                return InstantiateInternal(original, position, rotation, parent, worldPositionStays);
            }
            
            public static T InstantiateComponent<T>(bool hidden = false, bool dontDestroyOnLoad = false) where T : Component
            {
                var gameObject = new GameObject
                {
                    name = typeof(T).Name,
                    hideFlags = hidden ? HideFlags.HideInHierarchy | HideFlags.HideInInspector : HideFlags.None,
                };
                
                var component = gameObject.AddComponent<T>();
                
                if (dontDestroyOnLoad)
                    UnityEngine.Object.DontDestroyOnLoad(gameObject);
                
                return component;
            }


            private static T InstantiateInternal<T>(T original, Vector3 position, Quaternion rotation, Transform parent,
                bool worldPositionStays) where T : UnityEngine.Object
            {
                if (!GetActiveState(original))
                {
                    return UnityEngine.Object.Instantiate(original, position, rotation, parent);
                }

                var instance = UnityEngine.Object.Instantiate(original, position, rotation, HiddenCoreObject);

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
}