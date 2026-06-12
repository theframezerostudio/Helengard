using System;
using UnityEngine;

[Serializable]
public sealed class CombatProgressionRepetitionSettings
{
    [SerializeField] private bool enabled = true;
    [SerializeField] private float checkWindow = 4f;
    [SerializeField] private int allowedUses = 2;
    [SerializeField] private float repeatedPenalty = -15f;
    [SerializeField] private bool blockBaseScoreWhenRepeated;
    [SerializeField] private bool onlyPunishPositiveScore = true;

    public bool Enabled => enabled;
    public float CheckWindow => checkWindow;
    public int AllowedUses => allowedUses;
    public float RepeatedPenalty => repeatedPenalty;
    public bool BlockBaseScoreWhenRepeated => blockBaseScoreWhenRepeated;
    public bool OnlyPunishPositiveScore => onlyPunishPositiveScore;

    public void Validate()
    {
        if (checkWindow < 0.1f)
            checkWindow = 0.1f;

        if (allowedUses < 1)
            allowedUses = 1;

        if (repeatedPenalty > 0f)
            repeatedPenalty = -repeatedPenalty;
    }
}