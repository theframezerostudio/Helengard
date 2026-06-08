using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class CastingData
{
    public float horizontalMoveAmount;
    public float verticalMoveAmount;
}

[System.Serializable]
public class CastingStrategy
{
    [field: SerializeField] protected string StartAnimState {  get; private set; }
    [field: SerializeField] protected string ExecuteAnimState { get; private set; }
    [field: SerializeField] protected string RecoverAnimState { get; private set; }

    private PermissionManager permissionManager;
    protected CastingProperties properties;
    protected SpellAnimationController spellAnimator;
    private Coroutine recoveryRoutine = null;

    public void Initialize(CastingProperties properties, SpellAnimationController animator)
    {
        this.properties = properties;
        this.spellAnimator = animator;
        permissionManager = InputManager.Instance.permissionManager;
    }

    public virtual void Activate(SpellCastContext context)
    {
        EndRecovery();

        for (int i = 0; i < properties.blockAbilities.Length; i++)
        {
            permissionManager.Block(properties.blockAbilities[i]);
        }
    }

    public virtual void Performing(SpellCastContext context)
    {

    }

    public virtual void Deactivate()
    {
        for (int i = 0; i < properties.blockAbilities.Length; i++)
        {
            permissionManager.Release(properties.blockAbilities[i]);
        }
    }

    protected void StartRecovery(float duration, float transitionTime = 0.1f)
    {
        EndRecovery();

        recoveryRoutine = CoroutineManager.Run(RecoveryRoutine(duration, transitionTime));
    }

    protected void EndRecovery()
    {
        if (recoveryRoutine != null)
        {
            CoroutineManager.Stop(recoveryRoutine);
            recoveryRoutine = null;
        }
    }

    private IEnumerator RecoveryRoutine(float duration, float transitionTime = 0.1f)
    {
        permissionManager.BlockAll();

        yield return new WaitForSeconds(duration);
        float recDuration = spellAnimator.PlayAnim(RecoverAnimState, transitionTime);
        spellAnimator.SetIntent(0, recDuration);
        recoveryRoutine = null;

        permissionManager.ReleaseAll();
    }
}
