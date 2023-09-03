namespace TOAFL.Modules.StateMachine
{
    public interface IEnterState<in TArgument> : IState
    {
        void Enter(TArgument argument);
    }
}