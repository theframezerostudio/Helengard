using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [Header("Layers")]
    [SerializeField] private List<LayerState> layers;

    private AnimatorClipCache clipCache;
    private LayerStackResolver resolver;
    private Dictionary<string, LayerState> layerMap;

    private void Awake()
    {
        layerMap = new Dictionary<string, LayerState>();

        for (int i = 0; i < layers.Count; i++)
        {
            layers[i].Initialize(animator);
            layerMap[layers[i].layerName] = layers[i];
        }

        resolver = new LayerStackResolver(layers);
        clipCache = new AnimatorClipCache(animator);
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        resolver.Resolve(dt);

        ApplyToAnimator();
    }

    public void SetAnim(string anim, float value, float dampTime = 0f, int layer = 0, float intent = 1)
    {
        SetIntent(animator.GetLayerName(layer), intent);
        animator.SetFloat(anim, value, dampTime, Time.deltaTime);
    }

    public void SetAnim(string anim, bool value, int layer = 0, float intent = 1)
    {
        SetIntent(animator.GetLayerName(layer), intent);
        animator.SetBool(anim, value);
    }

    public void PlayAnim(string anim, float transitionTime = 0.1f, int layer = 0, float intent = 0)
    {
        SetIntent(animator.GetLayerName(layer), intent);
        animator.CrossFadeInFixedTime(anim, transitionTime, layer);
    }

    public float PlayAnim(string anim, float transitionTime = 0.1f, int layer = 0)
    {
        animator.CrossFade(anim, transitionTime, layer);
        
        return clipCache.GetDuration(animator.GetLayerName(layer), anim);
    }

    private void ApplyToAnimator()
    {
        for (int i = 0; i < layers.Count; i++)
        {
            LayerState layer = layers[i];
            animator.SetLayerWeight(layer.layerIndex, layer.weight);
        }
    }

    public void SetIntent(string layerName, float value)
    {
        if (layerMap.TryGetValue(layerName, out var layer))
        {
            layer.intent = Mathf.Clamp01(value);
        }
    }

    public Animator GetAnimator() => animator;
    public float GetDuration(string layerName, string animName) => clipCache.GetDuration(layerName, animName);
    public float GetWeight(string layerName)
    {
        return layerMap.TryGetValue(layerName, out var layer) ? layer.weight : 0f;
    }
}