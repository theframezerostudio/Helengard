using UnityEngine;

public sealed class CharacterAttributes : MonoBehaviour
{
    [SerializeField] private CharacterStatProfile profile;

    private StatContainer stats;
    private ResourceContainer resources;
    private GameplayEffectController effects;
    private AilmentController ailments;

    public StatContainer Stats => stats;
    public ResourceContainer Resources => resources;
    public GameplayEffectController Effects => effects;
    public AilmentController Ailments => ailments;

    private void Awake()
    {
        stats = new StatContainer(profile);
        resources = new ResourceContainer(profile.Resources, stats);
        effects = new GameplayEffectController(stats, resources);
        ailments = new AilmentController(effects, profile.AilmentResistances);
    }

    private void Update()
    {
        effects.Tick(Time.deltaTime);
        ailments.Tick(Time.deltaTime);
    }
}