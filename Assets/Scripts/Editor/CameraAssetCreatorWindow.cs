#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class CameraAssetCreatorWindow : EditorWindow
{
    private string assetName = "NewCameraAsset";
    private DefaultAsset targetFolder;

    private enum AssetKind
    {
        Mode,
        Modifier
    }

    private AssetKind assetKind = AssetKind.Mode;

    [MenuItem("Tools/Camera/Asset Creator")]
    public static void Open()
    {
        GetWindow<CameraAssetCreatorWindow>("Camera Asset Creator");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Create Camera Assets", EditorStyles.boldLabel);

        assetKind = (AssetKind)EditorGUILayout.EnumPopup("Asset Type", assetKind);
        assetName = EditorGUILayout.TextField("Asset Name", assetName);
        targetFolder = (DefaultAsset)EditorGUILayout.ObjectField("Target Folder", targetFolder, typeof(DefaultAsset), false);

        GUILayout.Space(10);

        if (GUILayout.Button("Create Asset"))
        {
            CreateAsset();
        }
    }

    private void CreateAsset()
    {
        string folderPath = "Assets";

        if (targetFolder != null)
        {
            folderPath = AssetDatabase.GetAssetPath(targetFolder);
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                EditorUtility.DisplayDialog("Invalid Folder", "Please assign a valid folder.", "OK");
                return;
            }
        }

        ScriptableObject asset = null;

        switch (assetKind)
        {
            case AssetKind.Mode:
                asset = CreateInstance<CameraModeDefinition>();
                break;

            case AssetKind.Modifier:
                asset = CreateInstance<CameraModifierDefinition>();
                break;
        }

        if (asset == null)
            return;

        string path = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/{assetName}.asset");
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeObject = asset;
        EditorGUIUtility.PingObject(asset);
    }
}
#endif