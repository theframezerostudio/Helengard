using UnityEngine;

[CreateAssetMenu(menuName = "AI/Combat Persona")]
public class CombatPersona : ScriptableObject
{
    [Header("Attack Core")]

    [Tooltip("Base desirability of attacking. Higher values make this persona naturally aggressive.")]
    public float attackBase = 5f;

    [Tooltip("Bonus score applied when Attack is first entered. Higher values increase combo commitment.")]
    public float attackEntryBonus = 3f;

    [Tooltip("Rate at which Attack score decays over time while active.")]
    public float attackDecayRate = 1f;


    [Header("Recover Core")]

    [Tooltip("Base desirability of entering Recovery state.")]
    public float recoverBase = 2f;

    [Tooltip("Bonus score applied when Recovery is first entered.")]
    public float recoverEntryBonus = 2f;

    [Tooltip("Rate at which Recovery score decays while active.")]
    public float recoverDecayRate = 0.5f;

    [Tooltip("Amount recovery score increases over time while not recovering.")]
    public float recoverTimeGrowth = 2f;


    [Header("Defend Core")]

    [Tooltip("Base desirability of defending.")]
    public float defendBase = 3f;

    [Tooltip("Bonus score applied when Defend is first entered.")]
    public float defendEntryBonus = 2f;

    [Tooltip("Rate at which Defend score decays while active.")]
    public float defendDecayRate = 1f;


    [Header("Dodge Core")]

    [Tooltip("Base desirability of dodging.")]
    public float dodgeBase = 3f;

    [Tooltip("Bonus score applied when Dodge is first entered.")]
    public float dodgeEntryBonus = 2f;

    [Tooltip("Rate at which Dodge score decays while active.")]
    public float dodgeDecayRate = 1.2f;


    // ---------------------------
    // ---------------------------
    // Context Modifiers          ====================
       [Space]                                        // ---------------------------
    // Context Modifiers          ====================
    // ---------------------------
    // ---------------------------

    [Header("Attack Context")]

    [Tooltip("Bonus added when AI is positioned behind the target.")]
    public float attackBackBonus = 3f;

    [Tooltip("Bonus added when the target is open.")]
    public float attackVsOpenBonus = 4f;

    [Tooltip("Bonus added when the target is blocking.")]
    public float attackVsBlockBonus = 2f;

    [Tooltip("Extra decay applied to Attack when the target is attacking.")]
    public float attackDecayVsThreat = 1.5f;

    [Tooltip("Decay reduction applied when an attack successfully hits.")]
    public float attackDecayOnHit = 1f;

    [Header("Recover")]

    [Tooltip("Decay on getting Hit by the Target")]
    public float recoverDecayOnHit = 0.5f;

    [Tooltip("Decay on Target Light Attack near AI")]
    public float recoverDecayOnLightAttacks = 0.2f;

    [Tooltip("Decay on Target Heavy Attack near AI")]
    public float recoverDecayOnHeavyAttacks = 0.3f;

    [Header("Defend Context")]

    [Tooltip("Bonus added when target uses light attack at close range in front.")]
    public float defendVsLightBonus = 4f;

    [Tooltip("Extra decay applied to Defend when the target is safe.")]
    public float defendDecayWhenSafe = 1.5f;

    [Tooltip("Decay reduction applied to Defend when the target is attacking.")]
    public float defendDecayWhenThreat = 1f;

    [Tooltip("Buildup for defense score based upon duration of stay in Attack State by Target.")]
    public float defenseAttackBuildupRate = 0.2f;

    [Tooltip("Max buildup for defense by Target stay in Attack State.")]
    public float defenseAttackBuildupMax = 3f;

    [Header("Dodge Context")]

    [Tooltip("Bonus added when the target uses heavy attack at close range in front.")]
    public float dodgeVsHeavyBonus = 5f;

    [Tooltip("Extra decay applied to Dodge when the target is safe.")]
    public float dodgeDecayWhenSafe = 1.5f;

    [Tooltip("Decay reduction applied to Dodge when the target is attacking.")]
    public float dodgeDecayWhenThreat = 1f; 
    
    [Tooltip("Buildup for defense score based upon duration of stay in Attack State by Target.")]
    public float dodgeAttackBuildupRate = 0.2f;

    [Tooltip("Max buildup for defense by Target stay in Attack State.")]
    public float dodgeAttackBuildupMax = 3f;
}