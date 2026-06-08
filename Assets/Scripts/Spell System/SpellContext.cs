using UnityEngine;

public sealed class SpellCastContext
{
    public CharacterContext CharacterContext { get; private set; }
    public CastingData CastingData { get; private set; }
    public SpellAimData Aim { get; private set; }
    public float DeltaTime { get; private set; }

    public void Set(CharacterContext characterContext, CastingData castingData, SpellAimData aim, float deltaTime)
    {
        CharacterContext = characterContext;
        CastingData = castingData;
        Aim = aim;
        DeltaTime = deltaTime;
    }
}