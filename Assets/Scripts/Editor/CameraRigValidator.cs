#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraRig))]
public class CameraRigValidator : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);

        if (GUILayout.Button("Validate Camera Rig"))
        {
            Validate((CameraRig)target);
        }
    }

    private void Validate(CameraRig rig)
    {
        if (rig == null)
            return;

        var serializedObject = new SerializedObject(rig);
        var bindingsProp = serializedObject.FindProperty("bindings");

        if (bindingsProp == null)
        {
            EditorUtility.DisplayDialog("Validation", "Could not find bindings list.", "OK");
            return;
        }

        int missingMode = 0;
        int missingCamera = 0;

        for (int i = 0; i < bindingsProp.arraySize; i++)
        {
            var entry = bindingsProp.GetArrayElementAtIndex(i);
            var modeProp = entry.FindPropertyRelative("mode");
            var camProp = entry.FindPropertyRelative("camera");

            if (modeProp.objectReferenceValue == null)
                missingMode++;

            if (camProp.objectReferenceValue == null)
                missingCamera++;
        }

        string message = $"Bindings checked.\nMissing Modes: {missingMode}\nMissing Cameras: {missingCamera}";
        EditorUtility.DisplayDialog("Camera Rig Validation", message, "OK");
    }
}
#endif