using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public virtual void Enter()
    {
        // Code to execute when entering the state
    }

    public virtual void UpdateState()
    {
        // Code to execute on each frame while in this state
    }

    public virtual void Exit()
    {
        // Code to execute when exiting the state
    }
}