using System;
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
    public EnemyLocomotionState IdleState;

    protected override void Awake()
    {
        base.Awake();

        stateMachine = GetComponent<EnemyStateMachine>();
    }

    private void Start()
    {
        IdleState = new EnemyLocomotionState(stateMachine, this);

        stateMachine.Initialize(IdleState);
    }
}
