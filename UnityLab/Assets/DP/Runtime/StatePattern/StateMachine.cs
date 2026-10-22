namespace Kyrnel
{
    public class StateMachine
    {
        private IState _currentState;

        public StateMachine(IState currentState)
        {
            _currentState = currentState;
            _currentState?.Enter();
        }

        public void ChangeState(IState newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState?.Enter();
        }

        public void Update()
        {
            _currentState?.Update();
        }
    }
}