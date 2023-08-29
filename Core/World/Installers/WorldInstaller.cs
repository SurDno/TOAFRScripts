using TOAFL.Core.Characters;
using UnityEngine;
using Zenject;

namespace TOAFL.Core.World
{
    public class WorldInstaller : MonoInstaller, IInitializable
    {
        [SerializeField] private PlayerStaticData playerStaticData;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<WorldInstaller>().FromInstance(this).AsSingle();
            Container.Bind<PlayerStaticData>().FromInstance(playerStaticData).AsSingle();
            Container.Bind<WorldConstructor>().To<WorldConstructor>().AsSingle();
        }

        public void Initialize()
        {
            var constructor = Container.Resolve<WorldConstructor>();
            
            constructor.Construct();
        }
    }
}