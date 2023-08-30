using UnityEngine;
using Zenject;

namespace TOAFL.Services.Camera.Installers
{
    public class CameraOperatorServiceInstaller : MonoInstaller
    {
        [SerializeField] private CameraOperatorStaticData cameraOperatorData;

        public override async void InstallBindings()
        {
            Container.Bind<CameraOperatorService>().ToSelf().AsSingle();
            Container.Bind<CameraOperatorStaticData>().FromInstance(cameraOperatorData).WhenInjectedInto<CameraOperatorService>();
        }
    }
}