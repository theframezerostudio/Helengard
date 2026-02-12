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

[CreateAssetMenu(fileName = "ComboNode", menuName = "Combo/ComboNode")]
public class ComboNode : ScriptableObject
{
    public AttackInput input;
    public MovementMotionPolicy motionPolicy;
    public RotationMotionPolicy rotationPolicy;
    public AttackProfile attackProfile;

    public AnimationCurve animMotionSpeed = AnimationCurve.Linear(0f, 1f, 1f, 1f);
    public string animationStateName;
    public AnimationClip animClip;
    public float transitionTime;

    public FrameWindow comboWindow;
    public FrameWindow attackWindow;
    public FrameWindow cancelWindow;
    public FrameWindow invincibleWindow;
    public FrameWindow moveWindow;

    public float attackTurnSpeed = 6f;          
    public AnimationCurve turnInfluence;        

    public float forwardAttackForce;
    public float upwardAttackForce;

    //public bool hasInputMotion;
    public bool requiresGround;
    public bool requiresAir;
    public bool requiresSprint;
    public bool requiresDash;

    [Header("Air Float")]
    public float amplitude = 0.05f;
    public float frequency = 2.5f;

    public List<ComboTransition> transitions;
}