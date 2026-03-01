using UnityEngine;

[System.Serializable]
public class AICombatMemory
{
    public CombatSubAction CurrentState;
    private float stateEnterTime;
    public float TimeInCurrentState => Time.time - stateEnterTime;

    public CombatSubAction PreviousState;
    public float TimeSincePreviousState;

    public float TimeSinceLastAttack;
    public float TimeSinceLastDefense;
    public float TimeSinceLastRecovery;
    public float TimeSinceLastDodge;

    public float ConsecutiveMissedAttacks;
    public float ConsecutiveSuccessfulAttacks;

    public float LastRecoverExitTime;

    public float TimeSinceAnyCombatStateChange;

    public void Tick(float deltaTime)
    {
        TimeSincePreviousState += deltaTime;

        TimeSinceLastAttack += deltaTime;
        TimeSinceLastDefense += deltaTime;
        TimeSinceLastRecovery += deltaTime;
        TimeSinceLastDodge += deltaTime;

        TimeSinceAnyCombatStateChange += deltaTime;
    }

    public void OnStateChanged(CombatSubAction newState)
    {
        TimeSinceAnyCombatStateChange = 0f;

        if (CurrentState != null)
        {
            PreviousState = CurrentState;
            TimeSincePreviousState = 0f;
        }

        CurrentState = newState;
        stateEnterTime = Time.time;

        if (newState is Attack_CombatAction)
            TimeSinceLastAttack = 0f;
        else if (newState is Defense_CombatAction)
            TimeSinceLastDefense = 0f;
        else if (newState is Recovery_CombatAction)
            TimeSinceLastRecovery = 0f;
        else if (newState is Dodge_CombatAction)
            TimeSinceLastDodge = 0f;
    }

    public void AttackConnected()
    {
        ConsecutiveSuccessfulAttacks++;
        ConsecutiveMissedAttacks = 0;
    }

    public void AttackMiised()
    {
        ConsecutiveMissedAttacks++;
        ConsecutiveSuccessfulAttacks = 0;
    }

    public void ResetAttackStreak()
    {
        ConsecutiveMissedAttacks = 0;
        ConsecutiveSuccessfulAttacks = 0;
    }

    public void ResetAll()
    {
        CurrentState = null;
        TimeSinceLastAttack = 0f;
        TimeSinceLastDefense = 0f;
        TimeSinceLastRecovery = 0f;
        TimeSinceLastDodge = 0f;
        ConsecutiveMissedAttacks = 0f;
        ConsecutiveSuccessfulAttacks = 0f;
        TimeSinceAnyCombatStateChange = 0f;
    }
}