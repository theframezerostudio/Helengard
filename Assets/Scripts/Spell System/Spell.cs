using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Spell/Create Spell")]
public class Spell : ScriptableObject
{
    [field: SerializeField] public string Label { get; protected set; }

    [SerializeReference, SubclassSelector] public CastingStrategy castingStrategy;
    public CastingProperties castingProperties;

    public void Initialize(SpellAnimationController animator)
    {
        castingStrategy.Initialize(castingProperties, animator);
    }

    public void Start()
    {
        castingStrategy.Start();
    }

    public void Tick(CastingData data)
    {
        castingStrategy.Performing(data);
    }

    public void Stop()
    {
        castingStrategy.Stop();
    }
}