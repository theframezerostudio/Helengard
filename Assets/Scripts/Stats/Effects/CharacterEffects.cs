using UnityEngine;

public sealed class CharacterEffects : MonoBehaviour
{
    [SerializeField] private CharacterAttributes characterStats;

    private GameplayEffectController effectController;

    public GameplayEffectController EffectController => effectController;

    public EffectDefinition TestEffect;

    private void Start()
    {
        effectController = new GameplayEffectController(characterStats.Stats, characterStats.Resources);
    }

    private void Update()
    {
        effectController.Tick(Time.deltaTime);
    }

    [ContextMenu("Test")]
    private void Test()
    {
        effectController.ApplyEffect(TestEffect);
    }
}