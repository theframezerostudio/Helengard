using UnityEngine;

public class EnemyState : BaseState
{
    protected readonly Enemy enemy;
    public EnemyState(StateMachine stateMachine, Character character) : base(stateMachine, character)
    {
        enemy = character as Enemy;
    }

    public override void Enter()
    {

    }

    public override void Update()
    {

    }

    public override void Exit()
    {

    }

    public override void LateUpdate()
    {

    }

    public override void OnTriggerEnter(Collider other)
    {

    }

    public override void OnTriggerExit(Collider other)
    {

    }

    public override void OnTriggerStay(Collider other)
    {

    }
}
