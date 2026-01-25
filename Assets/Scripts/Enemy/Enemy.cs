using System;
using UnityEngine;

public class Enemy : Character
{
    private EnemyStateMachine _stateMachine;

    [Header("States")]
    public EnemyIdleState IdleState;

    protected override void Awake()
    {
        base.Awake();

        _stateMachine = GetComponent<EnemyStateMachine>();
    }

    private void Start()
    {
        IdleState = new EnemyIdleState(_stateMachine, this);
    }
}
