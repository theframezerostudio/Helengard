using UnityEngine;

public abstract class AIAction : MonoBehaviour
{
    public abstract void Enter();
    public abstract void Tick();
    public abstract void Exit();
}
