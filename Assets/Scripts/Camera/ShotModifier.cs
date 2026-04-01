using UnityEngine;

public abstract class ShotModifier : MonoBehaviour
{
    public abstract void Initialize();
    public abstract void Enable();
    public abstract void Disable();
}
