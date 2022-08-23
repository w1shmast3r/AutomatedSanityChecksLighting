using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using UnityEditor;
using System.Collections;
using UnityEngine.Rendering;
using UnityEditor.UIAutomation;
using UnityEditor.SceneManagement;
using System;
using static Util;
using UnityEngine.SceneManagement;

public class RegressionChecks
{

    [UnityTest]
    public IEnumerator FallbackToCPUAfterSwitchingLightColor()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/FallbackToCPUAfterChangingLightColor/t1.unity");
        if (Lightmapping.isRunning == false)
        {
            //Prepare the scene's LSA and set Progressive GPU as a lightmapping backend
            var LSAdefault = Lightmapping.lightingSettings;
            var LSACur = LSAdefault;
            if (LSACur.lightmapper == LightingSettings.Lightmapper.ProgressiveCPU)
            {
                LSACur.lightmapper = LightingSettings.Lightmapper.ProgressiveGPU;
            }

            //Clear scene's baked data and GI Cache
            Lightmapping.Clear();
            Lightmapping.ClearDiskCache();

            //Bake GI
            Lightmapping.Bake();

            //Change the light color
            foreach (Light l in Resources.FindObjectsOfTypeAll<Light>())
                l.gameObject.GetComponent<Light>().color = Color.red;

            //Rebake GI
            Lightmapping.Bake();

            //Check that the lightmappper hasn't fallback to CPU
            Assert.AreEqual(Lightmapping.lightingSettings.lightmapper, LightingSettings.Lightmapper.ProgressiveGPU);

            //Clean up the scene and reset default LSA setttings
            foreach (Light l in Resources.FindObjectsOfTypeAll<Light>())
                l.gameObject.GetComponent<Light>().color = Color.white;
            Lightmapping.lightingSettings = LSAdefault;

        }
        yield return null;

    }

    [UnityTest]
    public IEnumerator CrashAfterBakingNewSceneAfterPreviousBakeWithGPUPLM()
    {
        if (Lightmapping.isRunning == false)
        {
            for (int i = 0; i < 2; i++)
            {
                EditorSceneManager.OpenScene("Assets/Scenes/TestScenes/CornellBoxCrashTest.unity");
                EditorSceneManager.SetActiveScene(EditorSceneManager.GetSceneByName("CornellBoxCrashTest"));
                var LSAdefault = Lightmapping.lightingSettings;
                var LSACur = LSAdefault;
                Lightmapping.Clear();
                Lightmapping.ClearDiskCache();
                Lightmapping.ClearLightingDataAsset();
                LSACur.lightmapResolution = 1;
                LSACur.lightmapper = LightingSettings.Lightmapper.ProgressiveCPU;
                if (LSACur.realtimeGI == true)
                {
                    LSACur.realtimeGI = false;
                }
                yield return null;
                Lightmapping.Bake();
                yield return null;
                LSACur.lightmapper = LightingSettings.Lightmapper.ProgressiveGPU;
                yield return null;
                Lightmapping.Bake();
                yield return null;
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
                yield return null;
                Lightmapping.Bake();
                yield return null;
                Lightmapping.lightingSettings = LSAdefault;
            }
        }
        yield return null;

    }

    [UnityTest]
    public IEnumerator CrashAfterBakingNewSceneAfterPreviousBakeWithGPUPLMRealtimeGI()
    {
        if (Lightmapping.isRunning == false)
        {
            for (int i = 0; i < 2; i++)
            {
                EditorSceneManager.OpenScene("Assets/Scenes/TestScenes/CornellBoxCrashTest.unity");
                EditorSceneManager.SetActiveScene(EditorSceneManager.GetSceneByName("CornellBoxCrashTest"));
                var LSAdefault = Lightmapping.lightingSettings;
                var LSACurRealtime = LSAdefault;
                Lightmapping.Clear();
                Lightmapping.ClearDiskCache();
                Lightmapping.ClearLightingDataAsset();
                LSACurRealtime.lightmapResolution = 1;
                LSACurRealtime.lightmapper = LightingSettings.Lightmapper.ProgressiveCPU;
                LSACurRealtime.realtimeGI = true;
                yield return null;
                Lightmapping.Bake();
                yield return null;
                LSACurRealtime.lightmapper = LightingSettings.Lightmapper.ProgressiveGPU;
                yield return null;
                Lightmapping.Bake();
                yield return null;
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
                yield return null;
                Lightmapping.Bake();
                yield return null;
                if (LSACurRealtime.realtimeGI == true)
                {
                    LSACurRealtime.realtimeGI = false;
                }
                Lightmapping.lightingSettings = LSAdefault;
            }
        }
        yield return null;

    }

    [UnityTest]
    public IEnumerator AssertionWhenLoadingScenesWithBakedData()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/LoadSceneBug/s1Blue.unity");
        EditorSceneManager.OpenScene("Assets/Scenes/LoadSceneBug/s1Red.unity", OpenSceneMode.Additive);
        yield return null;

    }

    [UnityTest]
    public IEnumerator AssertionAfterUnloadingSceneWithBakedData()
    {
        //Open the initial scene
        EditorSceneManager.OpenScene("Assets/Scenes/LoadSceneBug2/SampleScene_A.unity");

        // Clear baked data and cache
        clearAll();

        //Bake the scene GI
        Lightmapping.Bake();
        while (Lightmapping.isRunning)
            yield return null;

        //Load the second scene
        EditorSceneManager.OpenScene("Assets/Scenes/LoadSceneBug2/SampleScene_B.unity", OpenSceneMode.Additive);

        //Unload the initial scene with baked GI
        EditorSceneManager.CloseScene(SceneManager.GetSceneByName("SampleScene_A"), true);
        yield return null;

        //Bake the scene GI
        Lightmapping.Bake();
        while (Lightmapping.isRunning)
            yield return null;
    }


    [UnityTest]
    public IEnumerator OpenLightExplorerAndCreateNewScene()
    {
        var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SetActiveScene(newScene);
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), string.Join("/", "Assets/Scenes/TestScenes/AssertionTest.unity"));
        GameObject lightGameObject = new GameObject("Light");
        Light lightComp = lightGameObject.AddComponent<Light>();
        yield return null;
        EditorApplication.ExecuteMenuItem("Window/Rendering/Light Explorer");
        yield return null;
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
        //this part will be implemented once I clarify how to call internal API in the Edit mode tests
        //var lightingWindow = EditorWindow.GetWindow<LightingExplorerWindow>();
        //lightingWindow.Close();
        yield return null;
    }

    [UnityTest]
    public IEnumerator ProgressiveUpdatesTimerCheck()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/CornellBox.unity");

        var path = loadLightingSettings();

        // Clear baked data and cache
        clearAll();

        // Switch to default light
        foreach (Light l in Resources.FindObjectsOfTypeAll<Light>())
            l.gameObject.SetActive(l.type == LightType.Rectangle);

        //Change settings
        Lightmapping.lightingSettings.autoGenerate = true;

        Lightmapping.lightingSettings.directSampleCount = 32;
        Lightmapping.lightingSettings.indirectSampleCount = 32;
        Lightmapping.lightingSettings.environmentSampleCount = 32;

        Lightmapping.lightingSettings.lightmapResolution = 64;
        Lightmapping.lightingSettings.filteringMode = LightingSettings.FilterMode.Auto;

        Lightmapping.lightingSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveGPU;
        Lightmapping.lightingSettings.prioritizeView = false;

        DateTime startTime = DateTime.Now;

        // Bake with Progressive Updates off
        while (Lightmapping.isRunning)
        {
            if (Lightmapping.lightingSettings.lightmapper == LightingSettings.Lightmapper.ProgressiveCPU)
            {
                resetLightingSettings(path);
                Assert.Inconclusive("Lightmapper fell back to CPU");
            }

            yield return null;
        }

        clearAll();
        Lightmapping.lightingSettings.prioritizeView = true;
        int elapsed = (int) (DateTime.Now.Subtract(startTime).TotalSeconds);
        startTime = DateTime.Now;

        // Bake with Progressive Updates on
        while (Lightmapping.isRunning)
        {
            if ((DateTime.Now.Subtract(startTime).TotalSeconds) > elapsed * 2)
            {
                resetLightingSettings(path);
                Assert.Fail("Detected Bake Loop");
            }

            if (Lightmapping.lightingSettings.lightmapper == LightingSettings.Lightmapper.ProgressiveCPU)
            {
                resetLightingSettings(path);
                Assert.Inconclusive("Lightmapper fell back to CPU");
            }

            yield return null;
        }

        resetLightingSettings(path);
    }

    [Test]
    public void ApplyLightingSettingsOnMultipleScenes_SettingsRemain()
    {
        var scene1 = EditorSceneManager.OpenScene("Assets/Scenes/CornellBox.unity");
        var scene2 = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        LightingSettings lsa = new LightingSettings
        {
            bakedGI = false
        };
        UnityEngine.SceneManagement.Scene[] scenes = { scene1, scene2 };
        Lightmapping.SetLightingSettingsForScenes(scenes, lsa);
        Assert.AreEqual(Lightmapping.GetLightingSettingsForScene(scene1), lsa);
        Assert.AreEqual(Lightmapping.GetLightingSettingsForScene(scene2), lsa);
    }
}
