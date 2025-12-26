using UnityEngine;

public abstract class BaseState 
{
    protected StateMachine stateMachine;
    protected Character character;

    public virtual bool IsCancellable => true;
    public virtual int Priority => 0;
    public virtual bool IsCompleted { get; protected set; } = false;

    public BaseState(StateMachine stateMachine, Character character)
    {
        this.stateMachine = stateMachine;
        this.character = character;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
    public abstract void OnTriggerEnter(Collider other);
    public abstract void OnTriggerStay(Collider other);
    public abstract void OnTriggerExit(Collider other);
}
