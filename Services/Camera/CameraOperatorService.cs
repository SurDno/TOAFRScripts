using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TOAFL.Services.Camera
{
    public class CameraOperatorService
    {
        private readonly CameraOperatorStaticData _operatorStaticData;
        private CameraOperator _operator;
        
        public CameraOperatorService(CameraOperatorStaticData operatorStaticData)
        {
            _operatorStaticData = operatorStaticData;
        }

        public void CaptureObject(Transform follow, Transform lookAt)
        {
            _operator.CameraTracker.Follow = follow;
            _operator.CameraTracker.LookAt = lookAt;
        }

        public async UniTask LoadEquipment()
        {
            var operatorGameObject = await Addressables.InstantiateAsync(_operatorStaticData.CameraOperatorReference);
            _operator = operatorGameObject.GetComponent<CameraOperator>();
        }
    }
}