#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class CameraSceneSetupUtility
{
    [MenuItem("Tools/Camera/Create Camera Rig Root")]
    public static void CreateCameraRigRoot()
    {
        GameObject root = new ("CameraRig");
        root.AddComponent<CameraRig>();
        root.AddComponent<CameraModifierRegistry>();
        root.AddComponent<CameraChannelRegistry>();
        root.AddComponent<CameraResolver>();
        root.AddComponent<CameraRuntimeApplier>();

        Selection.activeGameObject = root;
        EditorGUIUtility.PingObject(root);
    }
}
#endif