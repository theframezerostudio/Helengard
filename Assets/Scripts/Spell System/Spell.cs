using UnityEngine;

public enum InputType
{
    Tap,
    Hold,
}

[CreateAssetMenu(fileName = "New Spell", menuName = "Spell/Create Spell")]
public class Spell : ScriptableObject
{
    [field: SerializeField] public string Label { get; protected set; }
    [field: SerializeField] public InputType InputType { get; protected set; }

    [Header("Casting Settings")]
    [SerializeReference, SubclassSelector] public CastingStrategy castingStrategy;
    [SerializeField] private SpellAimSettings aimSettings = new();

    public CastingProperties castingProperties;
    public SpellAimSettings AimSettings => aimSettings;

    public void Initialize(SpellAnimationController animator)
    {
        castingStrategy.Initialize(castingProperties, animator);
    }

    public void Activate(SpellCastContext context)
    {
        castingStrategy.Activate(context);
    }

    public void Tick(SpellCastContext context)
    {
        castingStrategy.Performing(context);
    }

    public void Deactivate(SpellCastContext context)
    {
        castingStrategy.Deactivate();
    }
} 