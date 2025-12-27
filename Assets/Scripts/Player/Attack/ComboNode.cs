using System.Collections.Generic;
using UnityEngine;

public enum AttackInput
{
    Light,
    Heavy,
    LightHold,
    HeavyHold
}

[CreateAssetMenu(fileName = "ComboNode", menuName = "Nodes/ComboNode")]
public class ComboNode : ScriptableObject
{
    public AttackInput input;
    public string animationStateName;
    public float transitionTIme;

    public FrameWindow comboWindow;
    public FrameWindow cancelWindow;
    public FrameWindow invulWindow;
    public FrameWindow moveWindow;

    public float forwardAttackForce;
    public float upwardAttackForce;

    public bool requiresGround;
    public bool requiresAir;
    public bool requiresSprint;
    public bool requiresDash;

    public List<ComboTransition> transitions;
}