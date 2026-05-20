using UnityEngine;

public abstract class EffectModuleDefinition : ScriptableObject
{
    public abstract EffectModule CreateModule();
}
