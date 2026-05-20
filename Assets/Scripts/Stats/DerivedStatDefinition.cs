using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "DerivedStat",
    menuName = "Gameplay/Stats/Derived Stat"
)]
public sealed class DerivedStatDefinition : StatDefinition
{
    [Serializable]
    public struct Contribution
    {
        public StatDefinition stat;
        public float multiplier;
    }

    [Header("Formula")]

    [SerializeField] private List<Contribution> contributions = new();

    public IReadOnlyList<Contribution> Contributions => contributions;

    public override bool IsDerived => true;

    public float Evaluate(IStatSource statSource)
    {
        float value = 0f;

        for (int i = 0; i < contributions.Count; i++)
        {
            Contribution contribution = contributions[i];

            if (contribution.stat == null)
                continue;

            value += statSource.GetValue(contribution.stat) * contribution.multiplier;
        }

        return value;
    }
}