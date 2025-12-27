using UnityEngine;
using UnityEngine.LowLevel;

public enum AirStateType
{
    Rising,
    Falling,
}

public class PlayerAirState : PlayerState
{
    public override int Priority => 5;

    private AirStateType AirState;
    private float gravity;
    private bool jump;
    private JumpProfile jumpProfile;
    private Vector3 airMoveDirection;

    public PlayerAirState(StateMachine stateMachine, Character character, JumpProfile jumpProfile = null) : base(stateMachine, character)
    {
        this.jumpProfile = jumpProfile;
        jump = jumpProfile == null;
    }

    public override void Enter()
    {
        base.Enter();

        AirState = AirStateType.Rising;
        
        if (jumpProfile)
        {
            player.PlayAnim(jumpProfile.jumpAnim.name);
            player.verticalVelocity = jumpProfile.jumpForce;
        }
        else
        {
            player.PlayAnim("Fall", 1f);
            AirState = AirStateType.Falling;
        }

        gravity = player.gravity;
        airMoveDirection = inputManager.MoveInput;
    }

    public override void Update()
    {
        base.Update();

        player.verticalVelocity += gravity * Time.deltaTime;
        player.verticalVelocity = Mathf.Max(player.verticalVelocity, player.terminalVelocity);

        if (player.verticalVelocity < 0 && AirState == AirStateType.Rising)
        {
            AirState = AirStateType.Falling;

            if (jumpProfile != null)
            {
                player.PlayAnim(jumpProfile.fallAnim.name, 0.2f);
            }
            else
            {
                player.PlayAnim("Fall", 0.2f);
            }
        }

        player.Move(Vector3.up, player.verticalVelocity);

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

    private void HandleLand()
    {
        if (stateMachine.IsTransitioningState) return;

        if (AirState == AirStateType.Falling && player.Context.isGrounded)
        {
            stateMachine.TransitionToState(new PlayerRecoveryState(stateMachine, player, player.ActionProvider.landing));
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
