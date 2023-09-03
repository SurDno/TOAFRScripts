namespace TOAFL.Modules.StateMachine
{
    public abstract class BaseZenState : IEnterState
    {
        public BaseZenState(StateMachine stateMachine)
        {
            stateMachine.RegisterState(this);
        }
        
        public abstract void Exit();
        public abstract void Enter();
    }
}