using UnityEngine;
using UnityEditor;

public class EnableStaticShadowCaster
{
    [MenuItem("Tools/Enable Static Shadow Caster")]
    static void EnableStaticShadowCasterOption()
    {
        Renderer[] renderers = Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None);

        int count = 0;

        foreach (Renderer rend in renderers)
        {
            // Ignore prefabs/assets outside scene
            if (!rend.gameObject.scene.IsValid())
                continue;

            // Enable Static Shadow Caster
            rend.staticShadowCaster = true;

            EditorUtility.SetDirty(rend);

            count++;
        }

        Debug.Log($"Enabled Static Shadow Caster on {count} objects.");
    }
}