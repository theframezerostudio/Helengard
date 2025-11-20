public class PlayerStateMachine
{
    public PlayerState currentState;

    public void InitializeState(PlayerState state)
    {
        currentState = state;
        currentState?.Enter();
    }

    public void ChangeState(PlayerState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }
}