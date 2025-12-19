using System;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerStateMachine))]
[RequireComponent(typeof(PlayerInputHandler))]
public class Player : Character
{
    [Header("Player States")]
    public PlayerIdleState IdleState;
    public PlayerMoveState MoveState;
    public PlayerGuardState GuardState;
    public PlayerAirState AirState;

    [Header("Locomotion Modes")]
    public LocomotionMode LocomotionMode { get; private set; }
    private FreeMoveMode freeMoveMode;
    private TargetLockMode targetLockMode;

    [Header("References")]
    public CharacterController Controller { get; private set; }
    private PlayerStateMachine stateMachine;
    public PlayerInputHandler InputHandler { get; private set; }
    [field: SerializeField] public CharacterContext Context { get; private set; }

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector3 groundCheck;
    [SerializeField] private float footLength;
    [SerializeField] private float rayLength = 0.2f;
    [SerializeField] private int groundRays;

    [Header("Movement Settings")]
    public float acceleration = 10f;
    public float deceleration = 15f;
    public float sprintSpeed = 12f;

    [Header("InAir Settings")]
    public float jumpForce = 8f;
    public float gravity = -9.8f;
    public float airControlPercent = 0.5f;

    [Header("Guard Settings")]
    public float perfectGuardWindow = 0.2f;

    private void Awake()
    {
        stateMachine = GetComponent<PlayerStateMachine>();
        Controller = GetComponent<CharacterController>();
        InputHandler = GetComponent<PlayerInputHandler>();
    }

    private void Start()
    {
        IdleState = new PlayerIdleState(stateMachine,this);
        MoveState = new PlayerMoveState(stateMachine, this);
        GuardState = new PlayerGuardState(stateMachine, this);
        AirState = new PlayerAirState(stateMachine, this);

        Context = new CharacterContext();
        InputHandler.Initialize(Context);

        freeMoveMode = new FreeMoveMode(this);

        LocomotionMode = freeMoveMode;
        stateMachine.InitializeState(IdleState);
    }

    private void Update()
    {
        IsGrounded();
    }
    public void EnabletargetLock(Target target)
    {
        targetLockMode = new TargetLockMode(this, target);
        LocomotionMode = targetLockMode;
    }

    public void DisableTargetLock()
    {
        LocomotionMode = freeMoveMode;
    }

    public bool IsGrounded()
    {
        for (int i = 0; i < groundRays; i++)
        {
            float t = (float)i / (groundRays - 1);
            Vector3 rayOrigin = transform.position + transform.forward * Mathf.Lerp(-footLength / 2, footLength / 2, t) + groundCheck;
            Ray ray = new (rayOrigin, Vector3.down);
            if (Physics.Raycast(ray, rayLength, groundLayer))
            {
                return true;
            }
        }

        return false;
    }

    public void Move(Vector3 dir, float speed)
    {
        Controller.Move(speed * Time.deltaTime * dir);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for (int i = 0; i < groundRays; i++)
        {
            float t = (float)i / (groundRays - 1);
            Vector3 rayOrigin = transform.position + transform.forward * Mathf.Lerp(-footLength / 2, footLength / 2, t) + groundCheck;
            Gizmos.DrawLine(rayOrigin, rayOrigin + Vector3.down * rayLength);
        }
    }
}
