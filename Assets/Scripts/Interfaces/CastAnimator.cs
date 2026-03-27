using UnityEngine;

public abstract class CastAnimator : MonoBehaviour
{ 
    public abstract void SetAnim(string anim, float value, float dampTime = 0f);
    public abstract void PlayAnim(string anim, float transitionTime = 0.1f);
}
