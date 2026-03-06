using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateMachine : MonoBehaviour
{
    [SerializeField] private Character Owner;
    [SerializeField] private StateContext StateContext;

    [Tooltip("How often to check for state transitions.")]
    [SerializeField] private float transitionCheckInterval = 0.1f;
 
    [Tooltip("Decisions that are evaluated irrespective of current state")]
    [SerializeField] private AIDecision[] globalDecisions;

    /// <summary>
    /// Gets or sets the collection of AI states available for the agent.
    /// </summary>
    /// <remarks>Each element in the array represents a distinct behavior or mode that the AI agent can enter.
    /// Priority is given from top to bottom.</remarks>
    [SerializeField] private AIState[] states;

    [SerializeField, ReadOnly] private AIState currentState = null;


    private readonly Dictionary<string, int> statesDict = new();
    private CharacterContext context;
    private WaitForSeconds transitionCheckWaitTime;

    private void Start()
    {
        if (states == null || states.Length == 0)
            enabled = false;

        Initialize(states[0]);
        context = Owner.Context;
        transitionCheckWaitTime = new WaitForSeconds(transitionCheckInterval);
    }

    /// <summary>
    /// Initializes the AI state machine to the specified state and prepares all associated decisions for execution.
    /// </summary>
    /// <remarks>This method sets up internal state lookup and ensures that all decisions within each AI state
    /// are properly initialized with the current owner and combat context. The specified state is entered immediately
    /// after initialization.</remarks>
    /// <param name="state">The AI state to set as the current state. Must not be null; this state will be entered and its decisions
    /// initialized.</param>
    private void Initialize(AIState state)
    {
        for (int i = 0; i < states.Length; i++)
        {
            // Build a dictionary for quick state lookup by label
            if (!string.IsNullOrEmpty(states[i].Label))
                statesDict[states[i].Label] = i;

            // Initialize decisions with owner and combat data
            foreach (AIDecision decision in states[i].Decisions)
            {
                decision.Initialize(Owner, StateContext.CombatData);
            }
        }

        for (int i = 0; i < globalDecisions.Length; i++)
        {
            globalDecisions[i].Initialize(Owner, StateContext.CombatData);
        }

        TransitionToState(state);

        // Initiate Transition checking routine to evaluate state changes based on decisions
        StartCoroutine(TransitionCheckRoutine());
    }

    private void Update()
    {
        currentState?.Tick();
    }

    private void LateUpdate()
    {
        Owner.motionAccumulator.Consume(out Vector3 moveDelta, out Quaternion rotDelta);

        Vector3 velocity = moveDelta / Time.deltaTime;

        Vector3 roundedVelocity = new Vector3(
        Mathf.Round(velocity.x * 100f) / 100f,
        Mathf.Round(velocity.y * 100f) / 100f,
        Mathf.Round(velocity.z * 100f) / 100f
        );

        context.Velocity = roundedVelocity;

        Owner.Controller.Move(moveDelta);
        Owner.transform.rotation = rotDelta * Owner.transform.rotation;
    }

    private IEnumerator TransitionCheckRoutine()
    {
        // TODO: If creating bottle necks,
        // consider spreading out global and regular transition checks across multiple frames

        while (enabled)
        {
            yield return transitionCheckWaitTime;
               
            CheckTransitions();

            yield return null;
        }
    }

    public void TransitionToState(AIState state)
    {
        if (state == null || state == currentState)
            return;

        if (currentState.IsLocked)
            return;

        currentState?.Exit();

        StateContext.SetState(state);
        currentState = state;

        currentState?.Enter(Owner, StateContext);
    }

    /// <summary>
    /// Checks for changing states based on current state decisions.
    /// Priority is based on the ordering of Decisions in the list.
    /// </summary>
    private bool CheckTransitions()
    {
        if (CheckGlobalTransitions())
            return true;

        foreach (AIDecision decision in currentState.Decisions)
        {
            string targetState = decision.ValidState();

            if (string.IsNullOrEmpty(targetState))
                continue;

            if (!statesDict.TryGetValue(targetState, out int index))
                continue;

            TransitionToState(states[index]);
            return true;
        }

        return false;
    }

    private bool CheckGlobalTransitions()
    {
        foreach (AIDecision decision in globalDecisions)
        {
            string targetState = decision.ValidState();

            if (string.IsNullOrEmpty(targetState))
                continue;

            if (!statesDict.TryGetValue(targetState, out int index))
                continue;

            TransitionToState(states[index]);
            return true;
        }

        return false;
    }
}
