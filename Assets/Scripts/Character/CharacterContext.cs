using System;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterContext : MonoBehaviour
{
    public AbilitySystem abilitySystem;

    public JumpResolver jumpResolver;
    public AttackResolver attackResolver;
    public MotionAccumulator MotionAccumulator { get; private set; } = new MotionAccumulator();

    public bool isSprinting;
    public bool isGrounded;
    public bool isDashing;

    //Serialized for testing purposes
    [SerializeField] private bool isLockedOn;
    [SerializeField] private bool isGuarding;
    public bool isPerfectGuarding;

    public Vector3 horizontalVelocity;
    public float UngroundedTime { get; private set; }

    public void UpdateGrounded(bool isGrounded, float deltaTime)
    {
        if (isGrounded)
        {
            this.isGrounded = true;
            UngroundedTime = 0f;
        }
        else
        {
            this.isGrounded = false;
            UngroundedTime += deltaTime;
        }
    }

    [Header("Character Context Events")]
    public bool IsGuarding
    {
        get { return isGuarding; }
        set
        {
            if (value == isGuarding) return;
            isGuarding = value;
            OnGuard?.Invoke(isGuarding);
        }
    }

    public bool IsLockedOn
    {
        get { return isLockedOn; }
        set
        {
            if (value == isLockedOn) return;

            isLockedOn = value;
            OnTargetLock?.Invoke(isLockedOn);
        }
    }

    public float GravityScale { get; internal set; }

    public Action<bool> OnTargetLock;
    public Action<bool> OnGuard;

    public void InitializeAbilities(AbilityData[] startingAbilities)
    {
        abilitySystem = new AbilitySystem(this, startingAbilities);
    }

    public bool CanDash() => abilitySystem.CanUse(AbilityType.Dash);
    public bool CanJump() => abilitySystem.CanUse(AbilityType.Jump);
}
