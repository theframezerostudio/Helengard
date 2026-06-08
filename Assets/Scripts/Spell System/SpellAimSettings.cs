using System;
using UnityEngine;

public enum SpellAimMode
{
    Manual,
    SoftTarget,
    LockedTarget,
    ActiveTarget,
    AutoTarget
}

public enum SpellAimUpdateMode
{
    ResolveOnStart,
    UpdateEveryTick
}

[Serializable]
public class SpellAimSettings
{
    public SpellAimMode aimMode = SpellAimMode.ActiveTarget;
    public SpellAimUpdateMode updateMode = SpellAimUpdateMode.UpdateEveryTick;

    public float targetRange = 12f;
    public float manualRange = 20f;

    public bool requireTarget;
    public bool allowManualFallback = true;

    public Vector3 castOffset = new Vector3(0f, 1.2f, 0.4f);
}