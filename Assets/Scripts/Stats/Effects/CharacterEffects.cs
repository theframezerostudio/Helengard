using UnityEngine;

namespace Stats
{
    public sealed class CharacterEffects : MonoBehaviour
    {
        // TODO: Requires Character Stats
        private GameplayEffectController gameplayEffects;

        public GameplayEffectController GameplayEffects => gameplayEffects;

        private void Awake()
        {
            // TODO: Replace placeholder for dependency injection or other initialization logic
        }

        private void Update()
        {
            gameplayEffects.Tick(Time.deltaTime);
        }
    }
}