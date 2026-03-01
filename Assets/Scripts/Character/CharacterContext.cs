using System;
using UnityEngine;

public class CharacterContext : MonoBehaviour
{
    public AbilitySystem abilitySystem;

    public DataAggregator dataAggregator;
    public JumpResolver jumpResolver;

    public float UngroundedTime { get; private set; }

    public bool isSprinting;
    public bool isGrounded;
    public bool isDashing;

    //Serialized for testing purposes
    [SerializeField] private bool isLockedOn;
    [SerializeField] private bool isGuarding;

    [HideInInspector] public bool isPerfectGuarding;
    [HideInInspector] public bool airComboDone = false;

    private Vector3 velocity;
    public Vector3 Velocity
    {
        get => velocity;
        set
        {
            dataAggregator.SetVelocity(value);
            velocity = value;
        }
    }

    public CombatSnapshot CombatData => dataAggregator.Snapshot;

    private void Awake()
    {
        dataAggregator = new DataAggregator();
    }

    public void UpdateGrounded(bool isGrounded, float deltaTime)
    {
        if (isGrounded)
        {
            this.isGrounded = true;
            UngroundedTime = 0f;
            airComboDone = false;
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
