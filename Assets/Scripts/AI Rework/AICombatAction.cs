using UnityEngine;

public enum CombatActionExitReason
{
    Completed,
    Replaced,
    Interrupted,
    CombatModeExited
}

/// <summary>
/// Runtime execution unit for one tactical combat behaviour.
///
/// Examples later:
/// - Attack execution
/// - Dodge execution
/// - Parry execution
/// - Maintain range execution
/// - Ability execution
///
/// This class does NOT decide whether it should be selected.
/// Selection/scoring will belong to a separate decision layer.
/// </summary>
public abstract class CombatAction : MonoBehaviour
{
    public Character Owner { get; private set; }
    public CombatBrain Brain { get; private set; }

    public bool IsRunning { get; private set; }

    /// <summary>
    /// Called by CombatBrain before checking or starting this action.
    /// Keeps execution ownership inside the brain.
    /// </summary>
    internal void Bind(CombatBrain brain, Character owner)
    {
        Brain = brain;
        Owner = owner;

        OnBound();
    }

    /// <summary>
    /// Optional validation before execution starts.
    /// This is only execution validity, not desirability scoring.
    ///
    /// Example:
    /// - Cannot attack without a weapon
    /// - Cannot dodge while already dead
    /// - Cannot cast an ability without its runtime dependency
    /// </summary>
    public virtual bool CanStart()
    {
        return true;
    }

    internal void Begin()
    {
        if (IsRunning)
            return;

        IsRunning = true;
        OnEnter();
    }

    internal void TickAction(float deltaTime)
    {
        if (!IsRunning)
            return;

        OnTick(deltaTime);
    }

    internal void End(CombatActionExitReason reason)
    {
        if (!IsRunning)
            return;

        IsRunning = false;
        OnExit(reason);
    }

    /// <summary>
    /// Allows the currently executing action to report natural completion.
    /// It does not directly choose another action.
    /// </summary>
    protected void Complete()
    {
        if (!IsRunning || Brain == null)
            return;

        Brain.NotifyActionCompleted(this);
    }

    /// <summary>
    /// Called whenever the brain supplies runtime ownership.
    /// Use only for lightweight reference binding.
    /// </summary>
    protected virtual void OnBound()
    {
    }

    protected abstract void OnEnter();

    protected abstract void OnTick(float deltaTime);

    protected abstract void OnExit(CombatActionExitReason reason);
}