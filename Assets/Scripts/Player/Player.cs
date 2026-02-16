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
    public PlayerDashState DashState;

    [Header("Locomotion Modes")]
    public LocomotionMode LocomotionMode { get; private set; }
    private FreeMoveMode freeMoveMode;
    private TargetLockMode targetLockMode;

    [Header("References")]
    public CharacterController Controller { get; private set; }
    [field: SerializeField] public LocomotionActionProvider ActionProvider { get; private set; }
    public PlayerInputHandler InputHandler { get; private set; }
    private PlayerStateMachine stateMachine;

    [Header("Abilities")]
    [SerializeField] private AbilityData[] startingAbilities;

    [Header("Movement Settings")]
    public float acceleration = 10f;
    public float deceleration = 15f;
    public float OppositeDotThreshold = -0.3f;
    public float customLength = 3f;
    public float sprintSpeed = 12f;
    public float horizontalVelocity = 0f;

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("InAir Settings")]
    public float jumpForce = 8f;
    public float airControlPercent = 0.5f;

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
        IdleState = new PlayerIdleState(stateMachine, this);
        MoveState = new PlayerMoveState(stateMachine, this);
        GuardState = new PlayerGuardState(stateMachine, this);
        DashState = new PlayerDashState(stateMachine, this);

        Context.InitializeAbilities(startingAbilities);
        InputHandler.Initialize(Context);

        freeMoveMode = new FreeMoveMode(this, motionAccumulator);

        LocomotionMode = freeMoveMode;
        stateMachine.Initialize(IdleState);

        verticalVelocity = groundSnapForce;
    }

    private void Update()
    {
        float vv = ApplyGravity(Time.deltaTime);

        LocomotionMode.AddImpulse(Vector3.up, vv);
    }

    public override void Suspend(float duration)
    {
        // Send Player to suspended state
    }

    public override void Recover(ActionData actionData)
    {
        // Send Player to recovery state based on actionData
    }

    public override void Unsuspend()
    {
        // Send Player back to idle or move state based on input or other conditions
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
}
