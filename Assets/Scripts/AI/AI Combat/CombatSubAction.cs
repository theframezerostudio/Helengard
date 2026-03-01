using UnityEngine;

public abstract class CombatSubAction : MonoBehaviour
{
    [field: SerializeField] public string Label { get; protected set; }
    public bool useLock;

    protected Character owner;

    protected StateContext stateContext;
    protected AICombatData combatData;
    protected AICombatMemory combatMemory;

    protected float stateTimer;

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

    public abstract float Evaluate(CombatPersona persona);
}