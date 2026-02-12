using UnityEngine;
using UnityEngine.AI;

public class Enemy : Character
{
    public Transform temporaryTargetForNow;



    private EnemyStateMachine stateMachine;

    public NavMeshAgent agent;
    public AgentMotionHandler motionHandler;
    public CharacterController controller;

    [Header("States")]
    public EnemyLocomotionState LocomotionState;

    public bool IsAlive => true; // Needs to be updated

    protected override void Awake()
    {
        base.Awake();

        stateMachine = GetComponent<EnemyStateMachine>();
    }

    private void Start()
    {
        LocomotionState = new EnemyLocomotionState(stateMachine, this);
        stateMachine.Initialize(LocomotionState);
        //stateMachine.Initialize(new EnemyState(stateMachine, this));
    }

    private void Update()
    {
        float vv = ApplyGravity(Time.deltaTime);

        motionAccumulator.AddExtraDelta(Vector3.up * vv);
    }

    public override void Suspend(float duration)
    {
        stateMachine.TransitionToState(new EnemySuspendedState(stateMachine, this, duration));
    }

    public override void Recover(ActionData actionData)
    {
        stateMachine.TransitionToState(new EnemyRecoveryState(stateMachine, this, actionData));
    }

    public override void Unsuspend()
    {
        // Unsuspend characeter
        stateMachine.TransitionToState(LocomotionState);
    }
}
