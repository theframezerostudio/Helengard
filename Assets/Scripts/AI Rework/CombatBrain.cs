using UnityEngine;

/// <summary>
/// Tactical runtime controller active only while the AI is in Combat mode.
///
/// Current responsibility:
/// - Own the active combat action lifecycle.
/// - Allow one action to replace or interrupt another safely.
///
/// Future responsibility:
/// - Receive selected actions from CombatSelector.
///
/// Not owned here yet:
/// - Perception building
/// - Memory
/// - Persona tuning
/// - Scoring
/// - Action categories/tags
/// </summary>
[DisallowMultipleComponent]
public sealed class CombatBrain : MonoBehaviour
{
    [Header("Temporary Bootstrapping")]
    [Tooltip(
        "Optional action started when Combat mode begins. " +
        "This exists only to test action execution before the selector is implemented.")]
    [SerializeField] private CombatAction startupAction;

    [Header("Runtime Debug")]
    [SerializeField, ReadOnly] private CombatAction currentAction;
    [SerializeField, ReadOnly] private bool isActive;

    public Character Owner { get; private set; }

    public CombatAction CurrentAction => currentAction;
    public bool IsActive => isActive;

    /// <summary>
    /// Called when the outer AI state machine enters Combat mode.
    /// </summary>
    public void Activate(Character owner)
    {
        if (owner == null)
        {
            Debug.LogError($"{nameof(CombatBrain)} cannot activate without an owner.", this);
            return;
        }

        if (isActive)
        {
            if (Owner == owner)
                return;

            Deactivate();
        }

        Owner = owner;
        isActive = true;

        if (startupAction != null)
        {
            TryStartAction(startupAction);
        }
    }

    /// <summary>
    /// Called only by the Combat-mode bridge action.
    /// CombatBrain intentionally does not use its own Update loop.
    /// </summary>
    public void Tick(float deltaTime)
    {
        if (!isActive)
            return;

        currentAction?.TickAction(deltaTime);
    }

    /// <summary>
    /// Called when the outer AI state machine leaves Combat mode.
    /// </summary>
    public void Deactivate()
    {
        if (!isActive)
            return;

        StopCurrentAction(CombatActionExitReason.CombatModeExited);

        Owner = null;
        isActive = false;
    }

    /// <summary>
    /// Entry point that a future CombatSelector will use.
    ///
    /// This method does not know what kind of action is being started.
    /// It may receive Attack, Dodge, Parry, Ability or any future behaviour.
    /// </summary>
    public bool TryStartAction(CombatAction nextAction)
    {
        if (!isActive || nextAction == null)
            return false;

        if (nextAction == currentAction)
            return true;

        nextAction.Bind(this, Owner);

        if (!nextAction.CanStart())
            return false;

        StopCurrentAction(CombatActionExitReason.Replaced);

        currentAction = nextAction;
        currentAction.Begin();

        return true;
    }

    /// <summary>
    /// Interrupts only the tactical combat action.
    /// It does not manipulate or lock the outer AI state machine.
    /// </summary>
    public void InterruptCurrentAction()
    {
        StopCurrentAction(CombatActionExitReason.Interrupted);
    }

    internal void NotifyActionCompleted(CombatAction completedAction)
    {
        if (completedAction == null || completedAction != currentAction)
            return;

        StopCurrentAction(CombatActionExitReason.Completed);
    }

    private void StopCurrentAction(CombatActionExitReason reason)
    {
        if (currentAction == null)
            return;

        CombatAction actionToStop = currentAction;
        currentAction = null;

        actionToStop.End(reason);
    }
}