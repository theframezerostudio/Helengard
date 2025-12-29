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
    public PlayerDashState DashState;

    [Header("Locomotion Modes")]
    public LocomotionMode LocomotionMode { get; private set; }
    private FreeMoveMode freeMoveMode;
    private TargetLockMode targetLockMode;

    [Header("References")]
    public CharacterController Controller { get; private set; }
    [field: SerializeField] public LocomotionActionProvider ActionProvider { get; private set; }
    public PlayerInputHandler InputHandler { get; private set; }
    [field: SerializeField] public CharacterContext Context { get; private set; }
    private PlayerStateMachine stateMachine;
    public MotionDriver MotionDriver;

    [Header("Abilities")]
    [SerializeField] private AbilityData[] startingAbilities;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector3 groundCheck;
    [SerializeField] private float footLength;
    [SerializeField] private float rayLength = 0.2f;
    [SerializeField] private int groundRays;

    [Header("Movement Settings")]
    //public float acceleration = 10f;
    //public float deceleration = 15f;
    public float sprintSpeed = 12f;
    public float horizontalVelocity = 0f;

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("InAir Settings")]
    public float jumpForce = 8f;
    public float gravity = -9.8f;
    public float airControlPercent = 0.5f;
    public float verticalVelocity = 0f;
    public float groundSnapForce = -12f;
    public float terminalVelocity = -20f;

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
        DashState = new PlayerDashState(stateMachine, this);

        Context.InitializeAbilities(startingAbilities);
        InputHandler.Initialize(Context);

        freeMoveMode = new FreeMoveMode(this);
        MotionDriver.Initialize(Context.MotionAccumulator);

        LocomotionMode = freeMoveMode;
        stateMachine.InitializeState(IdleState);
    }

    private void Update()
    {
        CheckGround();
        //ApplyGravity();
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

    public void CheckGround()
    {
        for (int i = 0; i < groundRays; i++)
        {
            float t = (float)i / (groundRays - 1);
            Vector3 rayOrigin = transform.position + transform.forward * Mathf.Lerp(-footLength / 2, footLength / 2, t) + groundCheck;
            Ray ray = new (rayOrigin, Vector3.down);
            if (Physics.Raycast(ray, rayLength, groundLayer))
            {
                Context.isGrounded = true;
                return;
            }
        }

        Context.isGrounded = false;
    }

    public void Move(Vector3 dir, float speed)
    {
        Controller.Move(speed * Time.deltaTime * dir);
    }

    public void DeltaMove(Vector3 delta)
    {
        Controller.Move(delta);
    }

    public void ApplyGravity()
    {
        if (Context.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = groundSnapForce;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
            verticalVelocity = Mathf.Max(verticalVelocity, terminalVelocity);
        }
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
