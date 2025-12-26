using UnityEngine;
using UnityEngine.Windows;

public class FreeMoveMode : LocomotionMode
{
    private readonly Camera mainCamera;

    public FreeMoveMode(Player player) : base(player)
    {
        mainCamera = Camera.main;
    }

    public override void Move(Vector3 input, float speed)
    {
        Vector3 dir = GetDirection(input).normalized;
        Vector3 velocity = speed * dir;

        player.Context.horizontalVelocity = velocity;

        player.Controller.Move(velocity * Time.deltaTime);
    }

    public override void Rotate(Vector2 input)
    {
        if (input.sqrMagnitude < 0.01f)
            return;

        Vector3 moveDir = GetDirection(input);

        Quaternion targetRot = Quaternion.LookRotation(moveDir);
        player.transform.rotation = Quaternion.Slerp(
            player.transform.rotation,
            targetRot,
            Time.deltaTime * player.rotationDamping
        );
    }

    public override void PerformDash(Vector2 dir)
    {
        Vector3 dashDir = GetDirection(dir);
        player.Controller.Move(dashDir * player.dashSpeed);
    }

    public override void PlayAnimation(Vector3 input)
    {
        player.SetAnim("Speed", input.magnitude);

        player.SetAnim("IsSprinting", player.Context.isSprinting);
    }

    public override void StopAnimation()
    {
        player.SetAnim("IsSprinting", false);
    }

    public override Vector3 GetDirection(Vector2 input)
    {
        Vector3 forward = (mainCamera.transform.forward).normalized;
        Vector3 right = (mainCamera.transform.right).normalized;

        forward.y = 0;
        right.y = 0;

        Vector3 dir = (input.x * right) + (input.y * forward);
        return dir;
    }
}