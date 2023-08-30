using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TOAFL.Services.Camera
{
    [CreateAssetMenu(menuName = "Scriptable/StaticData/CameraOperator", fileName = "CameraOperatorStaticData", order = 0)]
    public class CameraOperatorStaticData : ScriptableObject
    {
        [SerializeField] private AssetReferenceGameObject cameraOperatorReference;

        public AssetReferenceGameObject CameraOperatorReference => cameraOperatorReference;
    }
}