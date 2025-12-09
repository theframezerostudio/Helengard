using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] protected Animator animator;

    public float movementSpeed;
    public float rotationSpeed;
    public float rotationDamping;

    public void SetAnim(string anim, float value, float dampTime = 0f) => animator.SetFloat(anim, value, dampTime, Time.deltaTime);
    public void SetAnim(string anim, bool value) => animator.SetBool(anim, value);
    public void PlayAnim(string anim, float transitionTime = 0.1f) => animator.CrossFadeInFixedTime(anim, transitionTime);
}
