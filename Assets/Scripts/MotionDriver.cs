using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MotionDriver : MonoBehaviour
{
    [SerializeField] private Character character;
    private Animator _animator;
    private MotionAccumulator _motion;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _motion = character.motionAccumulator;
    }

    private void OnAnimatorMove()
    {
        _motion.AddDelta(_animator.deltaPosition);
        _motion.AddRotation(_animator.deltaRotation);
    }
}