using UnityEngine;

public abstract class LocomotionMode
{
    protected Player player;
    public Vector3 currentVelocity = Vector3.zero;
    protected Vector3 velocityHelper;

    public LocomotionMode(Player player)
    {
        this.player = player;
    }

    public void SetLocomotion(MovementMotionPolicy movementPolicy, RotationMotionPolicy rotationPolicy)
    {
        player.Context.MotionAccumulator.SetMotionData(movementPolicy, rotationPolicy, player.transform);
    }

    public abstract void Move(Vector3 dir, float movementSpeed);
    public abstract void Move(Vector2 input, float movementSpeed);
    public abstract void AddImpulse(Vector2 input, float distance);
    public abstract void AddImpulse(Vector3 dir, float distance);
    public abstract void Rotate(Vector2 dir);
    public abstract void PlayAnimation(Vector3 input);
    public abstract void StopAnimation();
    public abstract void PerformDash(Vector2 dir);
    public abstract Vector3 GetDirection(Vector2 input);
    public abstract void ResetVelocity();
}