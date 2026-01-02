using UnityEngine;
using UnityEngine.Windows;

public class FreeMoveMode : LocomotionMode
{
    private readonly Camera mainCamera;

    public FreeMoveMode(Player player) : base(player)
    {
        mainCamera = Camera.main;
    }

    public override void Move(Vector3 direction, float speed)
    {
        Vector3 velocity = speed * direction;

        currentVelocity = Vector3.SmoothDamp(currentVelocity, velocity, ref velocityHelper, 0.2f);
        player.Context.MotionAccumulator.AddExtraDelta(currentVelocity * Time.deltaTime);

        player.Context.horizontalVelocity = velocity;
    }

    public override void Move(Vector2 input, float movementSpeed)
    {
        Vector3 dir = GetDirection(input).normalized;
        Vector3 velocity = movementSpeed * dir;

        currentVelocity = Vector3.SmoothDamp(currentVelocity, velocity, ref velocityHelper, 0.2f);
        player.Context.MotionAccumulator.AddExtraDelta(currentVelocity * Time.deltaTime);

        player.Context.horizontalVelocity = velocity;
    }

    public override void Rotate(Vector2 input)
    {
        if (input.sqrMagnitude < 0.01f)
            return;

        Vector3 desiredDir = GetDirection(input);
        Quaternion desiredRotation = Quaternion.LookRotation(desiredDir);

        Quaternion current = player.transform.rotation;

        Quaternion deltaRotation = Quaternion.Inverse(current) * desiredRotation;

        deltaRotation = Quaternion.Slerp(
            Quaternion.identity,
            deltaRotation,
            Time.deltaTime * player.rotationDamping
        );

        player.Context.MotionAccumulator.AddRootRotation(deltaRotation);
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