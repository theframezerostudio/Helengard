using UnityEngine;

public abstract class CombatSubAction : MonoBehaviour
{
    [field: SerializeField] public string Label { get; protected set; }

    protected Character owner;

    protected StateContext stateContext;
    protected AICombatData combatData;
    protected AICombatMemory combatMemory;

    protected float stateTimer;

    [SerializeField, ReadOnly] private int lockCount;
    public bool IsLocked => lockCount > 0;

    public void Initialize(Character owner, StateContext context)
    {
        this.owner = owner;

        stateContext = context;
        combatData = context.CombatData;
        combatMemory = context.CombatMemory;
    }

    public virtual void Enter()
    {
        stateTimer = 0f;
    }

    public virtual void Tick()
    {
        stateTimer += Time.deltaTime;
    }

    public virtual void Exit() 
    {
        stateTimer = 0f;
    }

    public void Lock()
    {
        stateContext.State.Lock();
        lockCount++;
    }

    public void Unlock()
    {
        stateContext.State.Unlock();
        lockCount = Mathf.Max(0, lockCount - 1);
    }

    public abstract float Evaluate(CombatPersona persona);
}