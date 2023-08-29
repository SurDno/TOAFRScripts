using TOAFL.UserInput;
using Zenject;

namespace TOAFL.Services.UserInput
{
    public class InputServiceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<Controls>().To<Controls>().AsSingle();
        }
    }
}