using UnityEngine;

[CreateAssetMenu(fileName = "New AOE Spell", menuName = "Create Conjurations/AOE Conjurations")]
public class AOECastProperties : CastingProperties
{
    [Header("AOE Properties")]
    [field: SerializeField] public float circleMoveSpeed { get; private set; }
    [field: SerializeField] public float circleRange { get; private set; }
    [field: SerializeField] public float effectRadius { get; private set; }
    [field: SerializeField] public LayerMask groundMask { get; private set; }
}