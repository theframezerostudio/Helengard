using System;
using UnityEngine;

[Serializable]
public sealed class CombatProgressionMultiplierSettings
{
    [SerializeField] private bool enabled = true;
    [SerializeField] private float startingMultiplier = 1f;
    [SerializeField] private float maximumMultiplier = 2f;
    [SerializeField] private float gainPerMeaningfulAction = 0.05f;
    [SerializeField] private float lossOnPenalty = 0.15f;
    [SerializeField] private bool applyMultiplierToPenalties;
    [SerializeField] private bool resetOnMultiplierResetEvent = true;

    public bool Enabled => enabled;
    public float StartingMultiplier => startingMultiplier;
    public float MaximumMultiplier => maximumMultiplier;
    public float GainPerMeaningfulAction => gainPerMeaningfulAction;
    public float LossOnPenalty => lossOnPenalty;
    public bool ApplyMultiplierToPenalties => applyMultiplierToPenalties;
    public bool ResetOnMultiplierResetEvent => resetOnMultiplierResetEvent;

    public void Validate()
    {
        if (startingMultiplier < 1f)
            startingMultiplier = 1f;

        if (maximumMultiplier < startingMultiplier)
            maximumMultiplier = startingMultiplier;

        if (gainPerMeaningfulAction < 0f)
            gainPerMeaningfulAction = 0f;

        if (lossOnPenalty < 0f)
            lossOnPenalty = 0f;
    }
}