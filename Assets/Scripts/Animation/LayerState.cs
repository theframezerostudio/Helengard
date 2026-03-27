using UnityEngine;

[System.Serializable]
public class LayerState
{
    public string layerName;

    [Header("Runtime")]
    [Tooltip("Current Resolved Weight")]
    [ReadOnly] public float weight;
    [ReadOnly, Range(0f, 1f)]
    public float intent;     

    [Header("Behavior")]
    [Tooltip("Higher -> Evaluated first")]
    public int priority;     
    public float influence = 1f;

    public float fadeInSpeed = 5f;
    public float fadeOutSpeed = 10f;

    [HideInInspector] public int layerIndex;

    public void Initialize(Animator animator)
    {
        layerIndex = animator.GetLayerIndex(layerName);
    }

    public void UpdateWeight(float target, float dt)
    {
        float speed = (target > weight) ? fadeInSpeed : fadeOutSpeed;
        weight = Mathf.MoveTowards(weight, target, dt * speed);
    }
}