using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerStateMachine))]
public class Player : Character
{
    public PlayerIdleState IdleState;
    public PlayerMoveState MoveState;

    private CharacterController controller;
    private PlayerStateMachine stateMachine;

    public float acceleration = 10f;
    public float deceleration = 15f;

    private void Awake()
    {
        stateMachine = GetComponent<PlayerStateMachine>();
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        IdleState = new PlayerIdleState(stateMachine,this);
        MoveState = new PlayerMoveState(stateMachine, this);

        stateMachine.InitializeState(IdleState);
    }

    public void Move(Vector3 dir, float speed)
    {
        controller.SimpleMove(speed * dir);
    }
}
