using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Zenject;

namespace TOAFL.Modules.StateMachine
{
    [UsedImplicitly]
    public class StateMachine : ITickable
    {
        private readonly IDictionary<Type, IState> _statesPool;
        private IState _activeState;
        private IUpdatableState _updatableState;

        public StateMachine()
        {
            _statesPool = new Dictionary<Type, IState>();
        }

        public StateMachine(IDictionary<Type, IState> statesPool)
        {
            _statesPool = statesPool;
        }

        public void Enter<TState>() where TState : class, IEnterState
        {
            IEnterState enterState = ChangeState<TState>();
            enterState.Enter();
        }

        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IEnterState<TPayload>
        {
            TState state = ChangeState<TState>();
            state.Enter(payload);
        }
        
        public void Tick()
        {
            _updatableState?.Update();
        }

        internal void RegisterState(IEnterState state)
        {
            _statesPool.Add(state.GetType(), state);
        }

        private TState ChangeState<TState>() where TState : class, IState
        {
            _activeState?.Exit();

            TState state = GetState<TState>();
            _activeState = state;
            _updatableState = _activeState as IUpdatableState;

            return state;
        }

        private TState GetState<TState>() where TState : class, IState =>
            _statesPool[typeof(TState)] as TState;
    }
}