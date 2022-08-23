using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;
using UnityEditor.SceneManagement;
using NUnit.Framework;
using Unity.PerformanceTesting;
using Unity.PerformanceTesting.Runtime;
using static Util;

public class PerformanceChecks : MonoBehaviour
{
    void MeasureBakeWithBackend(string testLocation, LightingSettings.Lightmapper lightmapperType)
    {
        SampleGroup sampleGroup = new SampleGroup("Bake Time Raw", SampleUnit.Second);

        int kIterations = 3;

        for (int i = 0; i < kIterations; ++i)
        {
            // Prepare
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(testLocation + ".unity");

            LightingSettings lightingSettings = new LightingSettings();
            if (!Lightmapping.TryGetLightingSettings(out lightingSettings))
                lightingSettings = new LightingSettings();
            lightingSettings.lightmapper = lightmapperType;


            //LightingSettings.lightmapper = lightmapperType;

            UnityEditor.Lightmapping.ClearDiskCache();
            UnityEditor.Lightmapping.Clear();

            // Bake
            UnityEditor.Lightmapping.Bake();

            // Fetch timings
            Measure.Custom(sampleGroup, Utils.ConvertSample(SampleUnit.Second, sampleGroup.Unit, Lightmapping.GetLightmapBakeTimeRaw()));

            // Clean up
            UnityEditor.FileUtil.DeleteFileOrDirectory(testLocation);
            UnityEditor.AssetDatabase.Refresh();
        }
    }

    [Test, Performance, Version("1")]
    public void BakeLightingCPUPerformance()
    {
        MeasureBakeWithBackend("Assets/Scenes/CornellBox", LightingSettings.Lightmapper.ProgressiveCPU);
    }

    [Test, Performance, Version("1")]
    public void BakeLightingGPUPerformance()
    {
        MeasureBakeWithBackend("Assets/Scenes/CornellBox", LightingSettings.Lightmapper.ProgressiveGPU);
    }
}
