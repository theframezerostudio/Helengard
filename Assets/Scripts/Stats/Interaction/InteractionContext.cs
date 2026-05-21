using UnityEngine;

public sealed class InteractionContext
{
    public CharacterAttributes Source;
    public CharacterAttributes Target;

    public GameObject SourceObject;
    public object Causer;

    public Vector3 Position;
    public Vector3 Direction;

    public float PowerMultiplier = 1f;

    public CharacterAttributes Get(InteractionTarget target)
    {
        return target == InteractionTarget.Source ? Source : Target;
    }
}