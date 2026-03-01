using System;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private DamageReciever damageReciever;

    [Tooltip("Character Context of Character, if applicable")]
    [field: SerializeField] public CharacterContext Context { get; private set; }

    private void Start()
    {
        if (damageReciever == null)
        {
            Debug.LogWarning("DamageReciever not assigned on Target", this);
            return;
        }

        damageReciever.onDamageRecieved += HandleDamageRecieved;
    }

    private void HandleDamageRecieved()
    {
         Context.dataAggregator.MarkAsTargetted();
    }

    private void OnDestroy()
    {
        if (damageReciever != null)
            damageReciever.onDamageRecieved -= HandleDamageRecieved;
    }
}
