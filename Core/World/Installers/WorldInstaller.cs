using TOAFL.Core.Characters;
using UnityEngine;
using Zenject;

namespace TOAFL.Core.World
{
    public class WorldInstaller : MonoInstaller
    {
        [SerializeField] private PlayerStaticData playerStaticData;
        
        public override void InstallBindings()
        {
            Container.Bind<PlayerStaticData>().FromInstance(playerStaticData).AsSingle();
            Container.Bind<WorldConstructor>().ToSelf().AsSingle();
            Container.BindInterfacesTo<WorldRunner>().AsSingle();
            
            BindWorldStates();
            
        }

        private void BindWorldStates()
        {
            Container.Bind<WorldLoadState>().AsTransient().NonLazy();
            Container.Bind<WorldSetupState>().AsTransient().NonLazy();
            Container.Bind<WorldGameState>().AsTransient().NonLazy();
        }
    }
}