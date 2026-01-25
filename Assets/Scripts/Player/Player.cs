using System;
using System.Collections;
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

    [Header("Abilities")]
    [SerializeField] private AbilityData[] startingAbilities;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector3 groundCheck;
    [SerializeField] private float footLength;
    [SerializeField] private float rayLength = 0.2f;
    [SerializeField] private int groundRays;

    [Header("Movement Settings")]
    public float acceleration = 10f;
    public float deceleration = 15f;
    public float OppositeDotThreshold = -0.3f;
    public float minSlope = 0.7f;
    public float customLength = 3f;
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
    private bool hasGravity = true;

    [Header("Guard Settings")]
    public float perfectGuardWindow = 0.2f;

    protected override void Awake()
    {
        base.Awake();

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

        freeMoveMode = new FreeMoveMode(this, motionAccumulator);

        LocomotionMode = freeMoveMode;
        stateMachine.InitializeState(IdleState);

        verticalVelocity = groundSnapForce;
    }

    private void Update()
    {
        ApplyGravity(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    public void EnabletargetLock(Target target)
    {
        targetLockMode = new TargetLockMode(this, motionAccumulator, target);
        LocomotionMode = targetLockMode;
    }

    public void DisableTargetLock()
    {
        LocomotionMode = freeMoveMode;
    }

    private void ApplyGravity(float dt)
    {
        if (!hasGravity)
            return;

        if (Context.isGrounded)
        {
            if (verticalVelocity < 0f)
                verticalVelocity = groundSnapForce;
        }
        else
        {
            verticalVelocity += gravity * Context.GravityScale * dt;
            verticalVelocity = Mathf.Max(verticalVelocity, terminalVelocity);
        }

        LocomotionMode.AddImpulse(Vector3.up, verticalVelocity * dt);
    }

    public void CheckGround()
    {
        Vector3 origin = transform.position + groundCheck;

        float radius = footLength * 0.5f;
        float minSlopeDot = Mathf.Cos(minSlope * Mathf.Deg2Rad);

        bool grounded = false;

        for (int i = 0; i < groundRays; i++)
        {
            float angle = (360f / groundRays) * i;
            Vector3 offset =
                Quaternion.Euler(0, angle, 0) * Vector3.forward * radius;

            Vector3 rayOrigin = origin + offset;

            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, rayLength, groundLayer))
            {
                if (Vector3.Dot(hit.normal, Vector3.up) >= minSlopeDot)
                {
                    grounded = true;
                    break;
                }
            }

        }

        Context.UpdateGrounded(grounded, Time.fixedDeltaTime);
    }

    public void DeltaMove(Vector3 delta)
    {
        Controller.Move(delta);
    }

    public void DeltaRotate(Quaternion delta)
    {
        transform.rotation = delta * transform.rotation;
    }

    public void SetGravity(bool status)
    {
        hasGravity = status;
        if (!hasGravity)
        {
            //verticalVelocity = 0f;
        }
    }

    private void OnDrawGizmos()
    {
        if (groundRays <= 0) return;

        Vector3 origin = transform.position + groundCheck;
        float radius = footLength * 0.5f;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin, origin + Vector3.down * rayLength);

        Gizmos.color = Color.green;

        for (int i = 0; i < groundRays; i++)
        {
            float angle = (360f / groundRays) * i;
            Vector3 offset =
                Quaternion.Euler(0f, angle, 0f) * Vector3.forward * radius;

            Vector3 rayOrigin = origin + offset;
            Gizmos.DrawLine(rayOrigin, rayOrigin + Vector3.down * rayLength);
        }

        Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
        Gizmos.DrawWireSphere(origin, radius);
    }
}
