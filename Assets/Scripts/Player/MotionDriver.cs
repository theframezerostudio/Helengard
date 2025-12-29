using UnityEngine;

public class MotionDriver : MonoBehaviour
{
    public Animator animator;
    private MotionAccumulator motionAccumulator;

    public void Initialize(MotionAccumulator motionAccumulator)
    {
        this.motionAccumulator = motionAccumulator;
    }

    private void OnAnimatorMove()
    {
        motionAccumulator.AddRootDelta(animator.deltaPosition);
        motionAccumulator.AddRootRotation(animator.deltaRotation);
    }
}