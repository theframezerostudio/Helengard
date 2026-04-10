using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraRuntimeApplier : MonoBehaviour
{
    [SerializeField] private CameraModeController modeController;
    [SerializeField] private CameraRig cameraRig;
    [SerializeField] private CameraResolver resolver;

    private readonly Dictionary<CinemachineCamera, CameraComponentBinder> binderMap = new();

    private void Awake()
    {
        RebuildBinderMap();
    }

    private void OnEnable()
    {
        RebuildBinderMap();
    }

    private void LateUpdate()
    {
        if (modeController == null || cameraRig == null || resolver == null)
            return;

        var mode = modeController.CurrentMode;
        if (mode == null)
            return;

        CinemachineCamera activeCamera = cameraRig.GetCamera(mode);
        if (activeCamera == null)
            return;

        if (!binderMap.TryGetValue(activeCamera, out var binder) || binder == null)
            return;

        CameraResolvedState resolvedState = resolver.Resolve(mode);
        binder.Apply(resolvedState);
    }

    [ContextMenu("Rebuild Binder Map")]
    public void RebuildBinderMap()
    {
        binderMap.Clear();

        var binders = FindObjectsByType<CameraComponentBinder>(FindObjectsSortMode.None);

        foreach (var binder in binders)
        {
            if (binder == null || binder.Camera == null)
                continue;

            binderMap[binder.Camera] = binder;
        }
    }
}