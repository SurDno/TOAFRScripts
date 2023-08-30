using Cinemachine;
using UnityEngine;

namespace TOAFL.Services.Camera
{
    public class CameraOperator : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera cameraTracker;

        public CinemachineVirtualCamera CameraTracker => cameraTracker;
    }
}