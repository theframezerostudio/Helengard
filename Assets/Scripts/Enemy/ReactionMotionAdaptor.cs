using UnityEngine;

public class ReactionMotionAdapter
{
    private readonly MotionAccumulator accumulator;
    private readonly Transform transform;

    public ReactionMotionAdapter(Character character)
    {
        accumulator = character.motionAccumulator;
        transform = character.transform;
    }

    /// <summary>
    /// Push enemy in world-space direction (knockback, stagger slide)
    /// </summary>
    public void AddPositionDelta(Vector3 worldDelta)
    {
        accumulator.AddExtraDelta(worldDelta);
    }

    /// <summary>
    /// Push enemy in their local-space (forward/back/left/right)
    /// </summary>
    public void AddLocalPositionDelta(Vector3 localDelta)
    {
        accumulator.AddExtraDelta(transform.TransformDirection(localDelta));
    }

    /// <summary>
    /// Force a rotation delta (spin, turn-to-player, etc.)
    /// </summary>
    public void AddRotation(Quaternion delta)
    {
        accumulator.AddRotation(delta);
    }

    /// <summary>
    /// Set motion rules for this reaction,
    /// e.g. disable root-motion translation while knocked down.
    /// </summary>
    public void OverrideMotionPolicy(MovementMotionPolicy movePolicy, RotationMotionPolicy rotPolicy)
    {
        accumulator.SetMotionData(movePolicy, rotPolicy, transform);
    }

    /// <summary>
    /// Get current motion policies
    /// </summary>
    /// <param name="movePolicy"></param>
    /// <param name="rotPolicy"></param>
    public void GetMotionPolicy(out MovementMotionPolicy movePolicy, out RotationMotionPolicy rotPolicy)
    {
        accumulator.GetMotionData(out movePolicy, out rotPolicy);
    }
}