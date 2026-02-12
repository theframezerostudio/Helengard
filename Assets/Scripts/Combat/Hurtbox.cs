using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    public IDamageable owner;

    void Awake()
    {
        owner = GetComponentInParent<IDamageable>();
    }
}
