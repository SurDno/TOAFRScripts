using Zenject;

namespace TOAFL.Modules.StateMachine
{
    public class StateMachineInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<StateMachine>().ToSelf().AsSingle();
        }
    }
}