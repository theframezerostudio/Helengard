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
        character.motionAccumulator.Consume(out Vector3 moveDelta, out Quaternion rotDelta);
        enemy.controller.Move(moveDelta);
        enemy.transform.rotation = rotDelta * enemy.transform.rotation;
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
