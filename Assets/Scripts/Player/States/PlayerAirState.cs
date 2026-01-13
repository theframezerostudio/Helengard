using System.Collections;
using UnityEngine;
using UnityEngine.LowLevel;

public enum AirStateType
{
    Rising,
    Falling,
    Landing
}

public class PlayerAirState : PlayerState
{
    public override int Priority => 5;

    private AirStateType AirState;
    private float gravity;
    private readonly bool jump;
    private readonly JumpProfile jumpProfile;
    private Vector2 airMoveDirection;
    private float startTime;

    public PlayerAirState(StateMachine stateMachine, Character character, JumpProfile jumpProfile = null) : base(stateMachine, character)
    {
        this.jumpProfile = jumpProfile;
        jump = jumpProfile == null;
    }

    public override void Enter()
    {
        base.Enter();

        startTime = Time.time;
        AirState = AirStateType.Rising;
        
        if (jumpProfile)
        {
            player.PlayAnim(jumpProfile.jumpAnim.name);
            player.verticalVelocity = jumpProfile.jumpForce;
        }
        else
        {
            AirState = AirStateType.Falling;
            player.StartCoroutine(StartFall());
        }

        gravity = player.gravity;
        airMoveDirection = inputManager.MoveInput;
    }

    public override void Update()
    {
        base.Update();

        float dt = Time.deltaTime;
        if (dt <= 0f)
            return;

        float elapsed = Time.time - startTime;
        float gravityScale = jumpProfile ? jumpProfile.gravityCurve.Evaluate(elapsed) : 1f;
        player.verticalVelocity += player.gravity * gravityScale * dt;

        player.verticalVelocity = Mathf.Max(player.verticalVelocity, jumpProfile?.maxFallSpeed ?? -30f);

        if (player.verticalVelocity < 0 && AirState == AirStateType.Rising)
        {
            AirState = AirStateType.Falling;

            if (Time.deltaTime - startTime > 0.1f)
            {
                if (jumpProfile != null)
                {
                    player.PlayAnim(jumpProfile.fallAnim.name, 0.2f);
                }
                else
                {
                    player.PlayAnim("Fall", 0.2f);
                }
            }
        }

        player.LocomotionMode.AddImpulse(Vector3.up, player.verticalVelocity * dt);

        Vector2 movement = inputManager.MoveInput;

        if (jumpProfile)
        {
            player.LocomotionMode.Move(movement, player.movementSpeed * jumpProfile.airControlMultiplier);
            player.LocomotionMode.Move(airMoveDirection, jumpProfile.forwardForce);
        }
        else
        {
            player.LocomotionMode.Move(movement, player.movementSpeed * player.airControlPercent);
        }

        HandleLand();
    }

    private IEnumerator StartFall()
    {
        yield return new WaitForSeconds(0.1f);
        if (AirState == AirStateType.Falling)
            player.PlayAnim("Fall", 1f);
    }

    private void HandleLand()
    {
        if (AirState == AirStateType.Landing) return;

        if (AirState == AirStateType.Falling && player.Context.isGrounded)
        {
            Debug.Log("Landed, switching to Recovery State.");
            if (Time.deltaTime - startTime > 0.1f)
                stateMachine.TransitionToState(new PlayerRecoveryState(stateMachine, player, player.ActionProvider.landing));
            else
                SwitchToLocomotion();

            AirState = AirStateType.Landing;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
