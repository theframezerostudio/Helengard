using UnityEngine;
using UnityEngine.Windows;

public class FreeMoveMode : LocomotionMode
{
    private readonly Camera mainCamera;
    private float smoothingTime = 0.2f;

    public FreeMoveMode(Player player, MotionAccumulator motion) : base(player, motion)
    {
        mainCamera = Camera.main;
    }

    private float GetSmoothingDelta(float deltaTime, float timeConstant)
    {
        if (timeConstant <= 0f) return 1f;

        return 1f - Mathf.Exp(-deltaTime / timeConstant);
    }

    public override void Move(Vector3 direction, float speed)
    {
        float dt = Time.deltaTime;
        if (dt <= 0)
            return;

        Vector3 targetVelocity = speed * direction;

        if (direction.sqrMagnitude <= 0.001f || speed == 0f)
        {
            ResetVelocity();
            return;
        }

        float alpha = GetSmoothingDelta(dt, smoothingTime);

        Vector3 smoothTarget = Vector3.Lerp(currentVelocity, targetVelocity, alpha);
        currentVelocity = Vector3.MoveTowards(currentVelocity, smoothTarget, player.acceleration * dt);
        //currentVelocity = Vector3.SmoothDamp(currentVelocity, targetVelocity, ref velocityHelper, 0.2f);
        motion.AddExtraDelta(currentVelocity * dt);

        player.Context.horizontalVelocity = currentVelocity;
    }

    public override void Move(Vector2 input, float speed)
    {
        float dt = Time.deltaTime;
        if (dt <= 0)
            return;

        if (input.sqrMagnitude < 0.001f || speed == 0f)
        {
            ResetVelocity();
            return;
        }

        Vector3 dir = GetDirection(input).normalized;
        Vector3 targetVelocity = speed * dir;

        float alpha = GetSmoothingDelta(dt, smoothingTime);
        Vector3 smoothTarget = Vector3.Lerp(currentVelocity, targetVelocity, alpha);

        currentVelocity = Vector3.MoveTowards(currentVelocity, smoothTarget, player.acceleration * dt);

        motion.AddExtraDelta(currentVelocity * dt);
    }

    public override void AddImpulse(Vector2 input, float distance)
    {
        if (input.sqrMagnitude < 0.001f)
            return;

        Vector3 dir = GetDirection(input).normalized;
        Vector3 delta = distance * dir;

        //currentVelocity = Vector3.SmoothDamp(currentVelocity, delta, ref velocityHelper, 0.2f);
        motion.AddExtraDelta(delta);
    }

    public override void AddImpulse(Vector3 direction, float distance)
    {
        if (direction.sqrMagnitude < 0.001f)
            return;

        Vector3 delta = distance * direction;

        //currentVelocity = Vector3.SmoothDamp(currentVelocity, velocity, ref velocityHelper, 0.2f);
        motion.AddExtraDelta(delta);
    }

    public override void Rotate(Vector2 input)
    {
        if (input.sqrMagnitude < 0.01f)
            return;

        Vector3 desiredDir = GetDirection(input).normalized;
        Quaternion desiredRotation = Quaternion.LookRotation(desiredDir);
        Quaternion currentRotation = player.transform.rotation;

        Quaternion deltaRotation = Quaternion.Inverse(currentRotation) * desiredRotation;

        float dt = Time.deltaTime;
        if (dt <= 0f)
            return;

        float alpha = GetSmoothingDelta(dt, player.rotationTime );

        Quaternion smoothDelta = Quaternion.Slerp(
            Quaternion.identity,
            deltaRotation,
            alpha
        );

        motion.AddRotation(smoothDelta);
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

    public override void ResetVelocity()
    {
        currentVelocity = Vector3.zero;
        velocityHelper = Vector3.zero;
        player.Context.horizontalVelocity = Vector3.zero;
    }
}