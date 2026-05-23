using System;

[Flags]
public enum InteractionFlag
{
    None = 0,

    Hostile = 1 << 0,
    Blockable = 1 << 1,
    Parryable = 1 << 2,
    Evadable = 1 << 3,
    CanStagger = 1 << 4,
    CanKnockback = 1 << 5
}