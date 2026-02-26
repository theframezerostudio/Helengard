using UnityEngine;

public abstract class AIAction : MonoBehaviour
{
    protected Character owner;
    protected StateContext context;

    public abstract void Enter(Character Owner, StateContext stateContext);
    public abstract void Tick();
    public abstract void Exit();
}
