using NUnit.Framework.Interfaces;
using UnityEngine;

public class AbilityInstance
{
    public AbilityData data;
    public float lastUsedTime = float.NegativeInfinity;

    public AbilityInstance(AbilityData data)
    {
        this.data = data;
    }
}
