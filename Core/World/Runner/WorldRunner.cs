using TOAFL.Modules.StateMachine;
using Zenject;

namespace TOAFL.Core.World
{
    public class WorldRunner : IInitializable
    {
        private readonly StateMachine _stateMachine;

        public WorldRunner(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        
        public void Initialize()
        {
            _stateMachine.Enter<WorldLoadState>();
        }
    }
}