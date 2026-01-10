using UnityEngine;

public class Character : MonoBehaviour
{
    [field: SerializeField] public Animator Animator { get; protected set; }

    public float movementSpeed;
    public float rotationTime;

    public void SetAnim(string anim, float value, float dampTime = 0f) => Animator.SetFloat(anim, value, dampTime, Time.deltaTime);
    public void SetAnim(string anim, bool value) => Animator.SetBool(anim, value);
    public void PlayAnim(string anim, float transitionTime = 0.1f) => Animator.CrossFadeInFixedTime(anim, transitionTime, 0);
}
