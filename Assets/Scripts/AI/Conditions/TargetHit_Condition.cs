using System.Collections.Generic;
using UnityEngine;

// TODO: Better implement guard break instead of counting number of hits
public class TargetHit_Condition : Condition
{
    [Tooltip("The number of hits required to satisfy this condition.")]
    [SerializeField] private int requiredHits = 1;

    [Tooltip("Time in seconds after which hit count resets if no new hits are registered.")]
    [SerializeField] private float hitResetTime = 3f;

    [Tooltip("Target Component associated with this cahracter")]
    [SerializeField] private Target target;

    [SerializeField, ReadOnly] private int currHits = 0;

    private List<float> hitTimestamps = new();
    private float lastHitTime;

    public override void Initialize(Character owner, AICombatData combatData)
    {
        base.Initialize(owner, combatData);

        currHits = 0;
        lastHitTime = -hitResetTime; // Initialize to allow immediate hit registration

        target.onHit += RegisterHit;
    }

    public override bool Evaluate()
    {
        for (int i = hitTimestamps.Count - 1; i >= 0; i--)
        {
            if (Time.time - hitTimestamps[i] > hitResetTime)
            {
                hitTimestamps.RemoveAt(i);
                currHits--;
            }
        }

        return currHits >= requiredHits;
    }

    private void RegisterHit(DamageEvent ev)
    {
        // Reset hit count if hitResetTime has passed since the last hit
        if (Time.time - lastHitTime > hitResetTime)
        {
            currHits = 0;
        }

        currHits++;
        hitTimestamps.Add(Time.time);
        lastHitTime = Time.time;
    }

    private void OnDestroy()
    {
        if (target != null)
        {
            target.onHit -= RegisterHit;
        }
    }
}
