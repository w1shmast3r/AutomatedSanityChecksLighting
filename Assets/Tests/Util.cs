
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class Util
{
    public static string loadLightingSettings()
    {
        LightingSettings lightingSettings = new LightingSettings();
        if (!Lightmapping.TryGetLightingSettings(out lightingSettings))
            lightingSettings = new LightingSettings();

        string path = AssetDatabase.GetAssetPath(lightingSettings.GetInstanceID());
        AssetDatabase.CopyAsset(path, path + ".temp");

        return path;
    }

    public static void resetLightingSettings(string path)
    {
        var temp_path = path + ".temp";
        AssetDatabase.DeleteAsset(path);
        AssetDatabase.CopyAsset(temp_path, path);
        AssetDatabase.DeleteAsset(temp_path);

        var f = AssetDatabase.LoadAssetAtPath<LightingSettings>(path);
        Lightmapping.lightingSettings = f;
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    public static void clearAll()
    {
        Lightmapping.Clear();
        Lightmapping.ClearDiskCache();
        Lightmapping.ClearLightingDataAsset();
    }
}
