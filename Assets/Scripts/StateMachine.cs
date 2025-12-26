using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    protected BaseState CurrentState;
    protected BaseState QueuedState;

    public bool IsTransitioningState { get; private set; } = false;

    public void InitializeState(BaseState state)
    {
        CurrentState = state;
        CurrentState?.Enter();
    }

    private void Update()
    {
        CurrentState.Update();

        if (QueuedState != null)
        {
            TryApplyQueuedState();
        }
    }

    public void ForceState(BaseState newState)
    {
        TransitionToState(newState, true);
    }

    public void TransitionToState(BaseState newState, bool force = false)
    {
        if (IsTransitioningState || newState == null)
            return;

        if (CurrentState != null && !force)
        {
            if (!CurrentState.IsCancellable)
            {
                if (newState.Priority < CurrentState.Priority && !CurrentState.IsCompleted)
                {
                    QueuedState = newState;
                    return;
                }
            }
        }

        IsTransitioningState = true;

        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
        QueuedState = null;

        IsTransitioningState = false;
    }

    public void TryApplyQueuedState()
    {
        if (QueuedState != null)
        {
            TransitionToState(QueuedState);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CurrentState.OnTriggerEnter(other);
    }

    private void OnTriggerStay(Collider other)
    {
        CurrentState.OnTriggerStay(other);
    }

    private void OnTriggerExit(Collider other)
    {
        CurrentState.OnTriggerExit(other);
    }
}