using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Progression/Combat Progression Rule Set")]
public sealed class CombatProgressionRuleSet : ScriptableObject
{
    [SerializeField] private CombatProgressionRule[] rules;

    public int Count
    {
        get
        {
            if (rules == null)
                return 0;

            return rules.Length;
        }
    }

    public bool TryGetRule(int index, out CombatProgressionRule rule)
    {
        rule = null;

        if (rules == null)
            return false;

        if (index < 0 || index >= rules.Length)
            return false;

        rule = rules[index];

        return rule != null;
    }
}