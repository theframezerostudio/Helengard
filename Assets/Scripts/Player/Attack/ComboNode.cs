using System.Collections.Generic;
using UnityEngine;

public enum AttackInput
{
    Light,
    Heavy,
    LightHold,
    HeavyHold
}

public enum MovementMotionPolicy
{
    FullRootMotion,
    Root_XZ_MotionOnly,
    RootForwardOnly,
    NoRootMotion,
}

public enum RotationMotionPolicy
{
    FullRootRotation,
    YawOnly,
    NoRotation
}

[CreateAssetMenu(fileName = "ComboNode", menuName = "Nodes/ComboNode")]
public class ComboNode : ScriptableObject
{
    public AttackInput input;
    public MovementMotionPolicy motionPolicy;
    public RotationMotionPolicy rotationPolicy;
    public AnimationCurve animMotionSpeed = AnimationCurve.Linear(0f, 1f, 1f, 1f);
    public string animationStateName;
    public AnimationClip animClip;
    public float transitionTime;

    public FrameWindow comboWindow;
    public FrameWindow cancelWindow;
    public FrameWindow invincibleWindow;
    public FrameWindow moveWindow;

    public float forwardAttackForce;
    public float upwardAttackForce;

    //public bool hasInputMotion;
    public bool requiresGround;
    public bool requiresAir;
    public bool requiresSprint;
    public bool requiresDash;

    public List<ComboTransition> transitions;
}