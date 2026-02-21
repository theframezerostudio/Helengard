using System.Collections.Generic;
using UnityEngine;

public class AIStateMachine : MonoBehaviour
{
    [SerializeField] private Character Owner;
    [SerializeField] private AIState[] states;

    [SerializeField, ReadOnly] private AIState currentState = null;

    private Dictionary<string, int> statesDict = new();

    private void Start()
    {
        if (states == null || states.Length == 0)
            enabled = false;

        Initialize(states[0]);
    }

    private void Initialize(AIState state)
    {
        for (int i = 0; i < states.Length; i++)
        {
            if (!string.IsNullOrEmpty(states[i].Label))
                statesDict[states[i].Label] = i;
        }

        currentState = state;
        currentState?.Enter(Owner);
    }

    private void Update()
    {
        currentState?.Tick();
        CheckTransitions();
    }

    public void TransitionToState(AIState state)
    {
        currentState?.Exit();
        currentState = state;
        currentState?.Enter(Owner);
    }

    /// <summary>
    /// Checks for changing states based on current state decisions.
    /// Priority is based on the ordering of Decisions in the list.
    /// </summary>
    public void CheckTransitions()
    {
        foreach (AIDecision decision in currentState.Decisions)
        {
            string targetState = decision.ValidState();

            if (string.IsNullOrEmpty(targetState))
                continue;

            if (!statesDict.TryGetValue(targetState, out int index))
                continue;

            TransitionToState(states[index]);
            break;
        }
    }
}
