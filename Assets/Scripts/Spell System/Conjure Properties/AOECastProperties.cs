using UnityEngine;

[CreateAssetMenu(fileName = "New AOE Spell", menuName = "Create Conjurations/AOE Conjurations")]
public class AOECastProperties : CastingProperties
{
    [Header("AOE Properties")]
    public float circleMoveSpeed;
    public float circleRange;
}