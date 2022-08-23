using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;
using System.Linq;

namespace DeepTests
{
    public class Denoisers
    {

        public string loadLightingSettings()
        {
            LightingSettings lightingSettings = new LightingSettings();
            if (!Lightmapping.TryGetLightingSettings(out lightingSettings))
                lightingSettings = new LightingSettings();

            string path = AssetDatabase.GetAssetPath(lightingSettings.GetInstanceID());
            AssetDatabase.CopyAsset(path, path + ".temp");

            return path;
        }

        public void resetLightingSettings(string path)
        {
            var temp_path = path + ".temp";
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.CopyAsset(temp_path, path);
            AssetDatabase.DeleteAsset(temp_path);

            var f = AssetDatabase.LoadAssetAtPath<LightingSettings>(path);
            Lightmapping.lightingSettings = f;
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        }

        public void clearAll()
        {
            Lightmapping.Clear();
            Lightmapping.ClearDiskCache();
            Lightmapping.ClearLightingDataAsset();
        }


        [UnityTest]
        public IEnumerator BakeLightingGPU_Filtering_MixedDenoisers()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/CornellBox.unity");

            var path = loadLightingSettings();

            // Switch to default light
            foreach (Light l in Resources.FindObjectsOfTypeAll<Light>())
                l.gameObject.SetActive(l.type == LightType.Rectangle);

            // Generate all the possible denoiser combinations
            var denoiserCombinations = GetKCombsWithRept(new List<LightingSettings.DenoiserType>
            {
                LightingSettings.DenoiserType.None,
                LightingSettings.DenoiserType.RadeonPro,
                LightingSettings.DenoiserType.Optix,
                LightingSettings.DenoiserType.OpenImage
            }, 3);

            Lightmapping.lightingSettings.autoGenerate = true;

            // Run through all the combinations
            foreach (var combination in denoiserCombinations)
            {
                var c = combination.ToArray();
                clearAll();

                Lightmapping.lightingSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveGPU;
                Lightmapping.lightingSettings.filteringMode = LightingSettings.FilterMode.Advanced;

                // Change denoisers
                Lightmapping.lightingSettings.denoiserTypeDirect = c[0];
                Lightmapping.lightingSettings.denoiserTypeIndirect = c[1];
                Lightmapping.lightingSettings.denoiserTypeAO = c[2];

                // Bake
                while (Lightmapping.isRunning)
                {
                    if (Lightmapping.lightingSettings.lightmapper == LightingSettings.Lightmapper.ProgressiveCPU)
                    {
                        resetLightingSettings(path);
                        Assert.Inconclusive("Lightmapper fell back to CPU");
                    }

                    yield return null;
                }
            }

            resetLightingSettings(path);
        }

        [UnityTest]
        public IEnumerator BakeLightingCPU_Filtering_MixedDenoisers()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/CornellBox.unity");

            var path = loadLightingSettings();

            // Switch to default light
            foreach (Light l in Resources.FindObjectsOfTypeAll<Light>())
                l.gameObject.SetActive(l.type == LightType.Rectangle);

            // Generate all the possible denoiser combinations
            var denoiserCombinations = GetKCombsWithRept(new List<LightingSettings.DenoiserType>
            {
                LightingSettings.DenoiserType.None,
                LightingSettings.DenoiserType.RadeonPro,
                LightingSettings.DenoiserType.Optix,
                LightingSettings.DenoiserType.OpenImage
            }, 3);

            Lightmapping.lightingSettings.autoGenerate = true;

            // Run through all the combinations
            foreach (var combination in denoiserCombinations)
            {
                var c = combination.ToArray();
                clearAll();

                Lightmapping.lightingSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveCPU;
                Lightmapping.lightingSettings.filteringMode = LightingSettings.FilterMode.Advanced;

                // Change denoisers
                Lightmapping.lightingSettings.denoiserTypeDirect = c[0];
                Lightmapping.lightingSettings.denoiserTypeIndirect = c[1];
                Lightmapping.lightingSettings.denoiserTypeAO = c[2];

                // Bake
                while (Lightmapping.isRunning)
                    yield return null;
            }

            resetLightingSettings(path);
        }

        static IEnumerable<IEnumerable<T>>
            GetKCombsWithRept<T>(IEnumerable<T> list, int length) where T : IComparable
        {
            if (length == 1) return list.Select(t => new T[] { t });
            return GetKCombsWithRept(list, length - 1)
                .SelectMany(t => list.Where(o => o.CompareTo(t.Last()) >= 0),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }
    }
}
