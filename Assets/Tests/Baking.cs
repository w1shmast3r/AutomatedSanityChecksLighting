using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;
using UnityEditor.SceneManagement;
using NUnit.Framework;
using Unity.PerformanceTesting;
using Unity.PerformanceTesting.Runtime;
using static Util;

namespace SanityChecks
{
    public class Baking
    {
        [UnityTest]
        public IEnumerator BakeLightingCPU()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/CornellBox.unity");

            var path = loadLightingSettings();

            // Clear baked data and cache
            clearAll();

            // Switch to default light
            foreach (Light l in Resources.FindObjectsOfTypeAll<Light>())
                l.gameObject.SetActive(l.type == LightType.Rectangle);

            // Change settings
            Lightmapping.lightingSettings.autoGenerate = true;
            Lightmapping.lightingSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveCPU;

            // Bake
            while (Lightmapping.isRunning)
                yield return null;

            resetLightingSettings(path);
        }

        [UnityTest]
        public IEnumerator BakeLightingGPU()
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
            Lightmapping.lightingSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveGPU;

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

            resetLightingSettings(path);
        }

        [UnityTest]
        public IEnumerator BakeLightingGPU_Filtering_Auto()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/CornellBox.unity");

            var path = loadLightingSettings();

            // Clear baked data and cache
            clearAll();

            // Switch to default light
            foreach (Light l in Resources.FindObjectsOfTypeAll<Light>())
                l.gameObject.SetActive(l.type == LightType.Rectangle);

            // Change settings
            Lightmapping.lightingSettings.autoGenerate = true;
            Lightmapping.lightingSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveGPU;
            Lightmapping.lightingSettings.filteringMode = LightingSettings.FilterMode.Auto;

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

            resetLightingSettings(path);
        }

        [UnityTest]
        public IEnumerator BakeLightingGPU_Filtering_None()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/CornellBox.unity");

            var path = loadLightingSettings();
            // Clear baked data and cache
            clearAll();

            // Switch to default light
            foreach (Light l in Resources.FindObjectsOfTypeAll<Light>())
                l.gameObject.SetActive(l.type == LightType.Rectangle);

            // Change settings
            Lightmapping.lightingSettings.autoGenerate = true;
            Lightmapping.lightingSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveGPU;
            Lightmapping.lightingSettings.filteringMode = LightingSettings.FilterMode.None;

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

            resetLightingSettings(path);
        }

        [UnityTest]
        public IEnumerator BakeLightingGPU_Filtering_Optix()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/CornellBox.unity");

            var path = loadLightingSettings();

            // Switch to default light
            foreach (Light l in Resources.FindObjectsOfTypeAll<Light>())
                l.gameObject.SetActive(l.type == LightType.Rectangle);

            // Clear baked data and cache
            clearAll();

            // Change settings
            Lightmapping.lightingSettings.autoGenerate = true;
            Lightmapping.lightingSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveGPU;
            Lightmapping.lightingSettings.filteringMode = LightingSettings.FilterMode.Advanced;

            // Change all denoisers
            Lightmapping.lightingSettings.denoiserTypeDirect = LightingSettings.DenoiserType.Optix;
            Lightmapping.lightingSettings.denoiserTypeIndirect = LightingSettings.DenoiserType.Optix;
            Lightmapping.lightingSettings.denoiserTypeAO = LightingSettings.DenoiserType.Optix;

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

            resetLightingSettings(path);
        }

        [UnityTest]
        public IEnumerator BakeLightingGPU_Filtering_OpenImage()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/CornellBox.unity");

            var path = loadLightingSettings();

            // Switch to default light
            foreach (Light l in Resources.FindObjectsOfTypeAll<Light>())
                l.gameObject.SetActive(l.type == LightType.Rectangle);

            // Clear baked data and cache
            clearAll();

            // Change settings
            Lightmapping.lightingSettings.autoGenerate = true;
            Lightmapping.lightingSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveGPU;
            Lightmapping.lightingSettings.filteringMode = LightingSettings.FilterMode.Advanced;

            // Change all denoisers
            Lightmapping.lightingSettings.denoiserTypeDirect = LightingSettings.DenoiserType.OpenImage;
            Lightmapping.lightingSettings.denoiserTypeIndirect = LightingSettings.DenoiserType.OpenImage;
            Lightmapping.lightingSettings.denoiserTypeAO = LightingSettings.DenoiserType.OpenImage;

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

            resetLightingSettings(path);
        }

        [UnityTest]
        public IEnumerator BakeLightingGPU_Filtering_RadeonPro()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/CornellBox.unity");

            var path = loadLightingSettings();

            // Switch to default light
            foreach (Light l in Resources.FindObjectsOfTypeAll<Light>())
                l.gameObject.SetActive(l.type == LightType.Rectangle);

            // Clear baked data and cache
            clearAll();

            // Change settings
            Lightmapping.lightingSettings.autoGenerate = true;
            Lightmapping.lightingSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveGPU;
            Lightmapping.lightingSettings.filteringMode = LightingSettings.FilterMode.Advanced;

            // Change all denoisers
            Lightmapping.lightingSettings.denoiserTypeDirect = LightingSettings.DenoiserType.RadeonPro;
            Lightmapping.lightingSettings.denoiserTypeIndirect = LightingSettings.DenoiserType.RadeonPro;
            Lightmapping.lightingSettings.denoiserTypeAO = LightingSettings.DenoiserType.RadeonPro;

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

            resetLightingSettings(path);
        }

        [UnityTest]
        public IEnumerator BakeLightingGPU_Lights_PointLight()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/CornellBox.unity");

            var path = loadLightingSettings();

            // Clear baked data and cache
            clearAll();

            // Change settings
            Lightmapping.lightingSettings.autoGenerate = true;
            Lightmapping.lightingSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveGPU;

            // Switch to point light
            foreach (Light l in Resources.FindObjectsOfTypeAll<Light>())
                l.gameObject.SetActive(l.type == LightType.Point);

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

            resetLightingSettings(path);
        }

        [UnityTest]
        public IEnumerator BakeLightingGPU_Lights_DirectionalLight()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/CornellBox.unity");

            var path = loadLightingSettings();

            // Clear baked data and cache
            clearAll();

            // Change settings
            Lightmapping.lightingSettings.autoGenerate = true;
            Lightmapping.lightingSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveGPU;

            // Switch to directional light
            foreach (Light l in Resources.FindObjectsOfTypeAll<Light>())
                l.gameObject.SetActive(l.type == LightType.Directional);

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

            resetLightingSettings(path);
        }

        [UnityTest]
        public IEnumerator BakeLightingGPU_Lights_DiscLight()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/CornellBox.unity");

            var path = loadLightingSettings();

            // Clear baked data and cache
            clearAll();

            // Change settings
            Lightmapping.lightingSettings.autoGenerate = true;
            Lightmapping.lightingSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveGPU;

            // Switch to point light
            foreach (Light l in Resources.FindObjectsOfTypeAll<Light>())
                l.gameObject.SetActive(l.type == LightType.Disc);

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

            resetLightingSettings(path);
        }

        [UnityTest]
        public IEnumerator BakeLightingGPU_Lights_RectangleLight()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/CornellBox.unity");

            var path = loadLightingSettings();

            // Clear baked data and cache
            clearAll();

            // Change settings
            Lightmapping.lightingSettings.autoGenerate = true;
            Lightmapping.lightingSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveGPU;

            // Switch to point light
            foreach (Light l in Resources.FindObjectsOfTypeAll<Light>())
                l.gameObject.SetActive(l.type == LightType.Rectangle);

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

            resetLightingSettings(path);
        }

        [UnityTest]
        public IEnumerator BakeLightingGPU_Lights_Spotlight()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/CornellBox.unity");

            var path = loadLightingSettings();

            // Clear baked data and cache
            clearAll();

            // Change settings
            Lightmapping.lightingSettings.autoGenerate = true;
            Lightmapping.lightingSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveGPU;

            // Switch to point light
            foreach (Light l in Resources.FindObjectsOfTypeAll<Light>())
                l.gameObject.SetActive(l.type == LightType.Spot);

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

            resetLightingSettings(path);
        }

        [UnityTest]
        public IEnumerator BakeLightingGPU_ShadowMask()
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
            Lightmapping.lightingSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveGPU;
            Lightmapping.lightingSettings.mixedBakeMode = MixedLightingMode.Shadowmask;

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

            resetLightingSettings(path);
        }

        [UnityTest]
        public IEnumerator BakeLightingGPU_Subtractive()
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
            Lightmapping.lightingSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveGPU;
            Lightmapping.lightingSettings.mixedBakeMode = MixedLightingMode.Subtractive;

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

            resetLightingSettings(path);
        }

        [UnityTest]
        public IEnumerator BakeLightingGPU_BakedIndirect()
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
            Lightmapping.lightingSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveGPU;
            Lightmapping.lightingSettings.mixedBakeMode = MixedLightingMode.IndirectOnly;

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

            resetLightingSettings(path);
        }

        [UnityTest]
        public IEnumerator BakeLightingGPU_Emissive_Baked()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/CornellBox.unity");

            var path = loadLightingSettings();

            // Clear baked data and cache
            clearAll();

            // Turn all lights off
            foreach (Light l in Resources.FindObjectsOfTypeAll<Light>())
                l.gameObject.SetActive(false);

            // Switch to default light
            foreach (GameObject g in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                MeshRenderer renderer = g.GetComponent<MeshRenderer>();
                
                if (renderer != null && renderer.sharedMaterial.globalIlluminationFlags.HasFlag(MaterialGlobalIlluminationFlags.BakedEmissive))
                {
                    Debug.Log(renderer.sharedMaterial.globalIlluminationFlags);
                    renderer.gameObject.SetActive(true);
                }
            }

            //Change settings
            Lightmapping.lightingSettings.autoGenerate = true;
            Lightmapping.lightingSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveGPU;

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

        [UnityTest]
        public IEnumerator BakeLightingGPU_Probes()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/TestScenes/CornellBox_Probes.unity");

            var path = loadLightingSettings();

            // Clear baked data and cache
            clearAll();

            //Change settings
            Lightmapping.lightingSettings.autoGenerate = true;
            Lightmapping.lightingSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveGPU;

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

        /* ######################################################################################################################################
                                                              CPU LIGHTMAPPER TESTS
           ######################################################################################################################################
        */

        [UnityTest]
        public IEnumerator BakeLightingCPU_Filtering_Auto()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/CornellBox.unity");

            var path = loadLightingSettings();

            // Clear baked data and cache
            clearAll();

            // Switch to default light
            foreach (Light l in Resources.FindObjectsOfTypeAll<Light>())
                l.gameObject.SetActive(l.type == LightType.Rectangle);

            // Change settings
            Lightmapping.lightingSettings.autoGenerate = true;
            Lightmapping.lightingSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveCPU;
            Lightmapping.lightingSettings.filteringMode = LightingSettings.FilterMode.Auto;

            // Bake
            while (Lightmapping.isRunning)
                yield return null;

            resetLightingSettings(path);
        }

        [UnityTest]
        public IEnumerator BakeLightingCPU_Filtering_None()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/CornellBox.unity");

            var path = loadLightingSettings();
            // Clear baked data and cache
            clearAll();

            // Switch to default light
            foreach (Light l in Resources.FindObjectsOfTypeAll<Light>())
                l.gameObject.SetActive(l.type == LightType.Rectangle);

            // Change settings
            Lightmapping.lightingSettings.autoGenerate = true;
            Lightmapping.lightingSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveCPU;
            Lightmapping.lightingSettings.filteringMode = LightingSettings.FilterMode.None;

            // Bake
            while (Lightmapping.isRunning)
                yield return null;

            resetLightingSettings(path);
        }

        [UnityTest]
        public IEnumerator BakeLightingCPU_Filtering_Optix()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/CornellBox.unity");

            var path = loadLightingSettings();

            // Switch to default light
            foreach (Light l in Resources.FindObjectsOfTypeAll<Light>())
                l.gameObject.SetActive(l.type == LightType.Rectangle);

            // Clear baked data and cache
            clearAll();

            // Change settings
            Lightmapping.lightingSettings.autoGenerate = true;
            Lightmapping.lightingSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveCPU;
            Lightmapping.lightingSettings.filteringMode = LightingSettings.FilterMode.Advanced;

            // Change all denoisers
            Lightmapping.lightingSettings.denoiserTypeDirect = LightingSettings.DenoiserType.Optix;
            Lightmapping.lightingSettings.denoiserTypeIndirect = LightingSettings.DenoiserType.Optix;
            Lightmapping.lightingSettings.denoiserTypeAO = LightingSettings.DenoiserType.Optix;

            // Bake
            while (Lightmapping.isRunning)
                yield return null;

            resetLightingSettings(path);
        }

        [UnityTest]
        public IEnumerator BakeLightingCPU_Filtering_OpenImage()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/CornellBox.unity");

            var path = loadLightingSettings();

            // Switch to default light
            foreach (Light l in Resources.FindObjectsOfTypeAll<Light>())
                l.gameObject.SetActive(l.type == LightType.Rectangle);

            // Clear baked data and cache
            clearAll();

            // Change settings
            Lightmapping.lightingSettings.autoGenerate = true;
            Lightmapping.lightingSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveCPU;
            Lightmapping.lightingSettings.filteringMode = LightingSettings.FilterMode.Advanced;

            // Change all denoisers
            Lightmapping.lightingSettings.denoiserTypeDirect = LightingSettings.DenoiserType.OpenImage;
            Lightmapping.lightingSettings.denoiserTypeIndirect = LightingSettings.DenoiserType.OpenImage;
            Lightmapping.lightingSettings.denoiserTypeAO = LightingSettings.DenoiserType.OpenImage;

            // Bake
            while (Lightmapping.isRunning)
                yield return null;

            resetLightingSettings(path);
        }

        [UnityTest]
        public IEnumerator BakeLightingCPU_Filtering_RadeonPro()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/CornellBox.unity");

            var path = loadLightingSettings();

            // Switch to default light
            foreach (Light l in Resources.FindObjectsOfTypeAll<Light>())
                l.gameObject.SetActive(l.type == LightType.Rectangle);

            // Clear baked data and cache
            clearAll();

            // Change settings
            Lightmapping.lightingSettings.autoGenerate = true;
            Lightmapping.lightingSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveCPU;
            Lightmapping.lightingSettings.filteringMode = LightingSettings.FilterMode.Advanced;

            // Change all denoisers
            Lightmapping.lightingSettings.denoiserTypeDirect = LightingSettings.DenoiserType.RadeonPro;
            Lightmapping.lightingSettings.denoiserTypeIndirect = LightingSettings.DenoiserType.RadeonPro;
            Lightmapping.lightingSettings.denoiserTypeAO = LightingSettings.DenoiserType.RadeonPro;

            // Bake
            while (Lightmapping.isRunning)
                yield return null;

            resetLightingSettings(path);
        }

        [UnityTest]
        public IEnumerator BakeLightingCPU_Lights_PointLight()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/CornellBox.unity");

            var path = loadLightingSettings();

            // Clear baked data and cache
            clearAll();

            // Change settings
            Lightmapping.lightingSettings.autoGenerate = true;
            Lightmapping.lightingSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveCPU;

            // Switch to point light
            foreach (Light l in Resources.FindObjectsOfTypeAll<Light>())
                l.gameObject.SetActive(l.type == LightType.Point);

            // Bake
            while (Lightmapping.isRunning)
                yield return null;

            resetLightingSettings(path);
        }

        [UnityTest]
        public IEnumerator BakeLightingCPU_Lights_DirectionalLight()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/CornellBox.unity");

            var path = loadLightingSettings();

            // Clear baked data and cache
            clearAll();

            // Change settings
            Lightmapping.lightingSettings.autoGenerate = true;
            Lightmapping.lightingSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveCPU;

            // Switch to directional light
            foreach (Light l in Resources.FindObjectsOfTypeAll<Light>())
                l.gameObject.SetActive(l.type == LightType.Directional);

            // Bake
            while (Lightmapping.isRunning)
                yield return null;

            resetLightingSettings(path);
        }

        [UnityTest]
        public IEnumerator BakeLightingCPU_Lights_DiscLight()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/CornellBox.unity");

            var path = loadLightingSettings();

            // Clear baked data and cache
            clearAll();

            // Change settings
            Lightmapping.lightingSettings.autoGenerate = true;
            Lightmapping.lightingSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveCPU;

            // Switch to point light
            foreach (Light l in Resources.FindObjectsOfTypeAll<Light>())
                l.gameObject.SetActive(l.type == LightType.Disc);

            // Bake
            while (Lightmapping.isRunning)
                yield return null;

            resetLightingSettings(path);
        }

        [UnityTest]
        public IEnumerator BakeLightingCPU_Lights_RectangleLight()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/CornellBox.unity");

            var path = loadLightingSettings();

            // Clear baked data and cache
            clearAll();

            // Change settings
            Lightmapping.lightingSettings.autoGenerate = true;
            Lightmapping.lightingSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveCPU;

            // Switch to point light
            foreach (Light l in Resources.FindObjectsOfTypeAll<Light>())
                l.gameObject.SetActive(l.type == LightType.Rectangle);

            // Bake
            while (Lightmapping.isRunning)
                yield return null;

            resetLightingSettings(path);
        }

        [UnityTest]
        public IEnumerator BakeLightingCPU_Lights_Spotlight()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/CornellBox.unity");

            var path = loadLightingSettings();

            // Clear baked data and cache
            clearAll();

            // Change settings
            Lightmapping.lightingSettings.autoGenerate = true;
            Lightmapping.lightingSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveCPU;

            // Switch to point light
            foreach (Light l in Resources.FindObjectsOfTypeAll<Light>())
                l.gameObject.SetActive(l.type == LightType.Spot);

            // Bake
            while (Lightmapping.isRunning)
                yield return null;

            resetLightingSettings(path);
        }

        [UnityTest]
        public IEnumerator BakeLightingCPU_Emissive_Baked()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/CornellBox.unity");

            var path = loadLightingSettings();

            // Clear baked data and cache
            clearAll();

            // Turn all lights off
            foreach (Light l in Resources.FindObjectsOfTypeAll<Light>())
                l.gameObject.SetActive(false);

            // Switch to default light
            foreach (GameObject g in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                MeshRenderer renderer = g.GetComponent<MeshRenderer>();

                if (renderer != null && renderer.sharedMaterial.globalIlluminationFlags.HasFlag(MaterialGlobalIlluminationFlags.BakedEmissive))
                {
                    Debug.Log(renderer.sharedMaterial.globalIlluminationFlags);
                    renderer.gameObject.SetActive(true);
                }
            }

            //Change settings
            Lightmapping.lightingSettings.autoGenerate = true;
            Lightmapping.lightingSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveCPU;

            // Bake
            while (Lightmapping.isRunning)
                yield return null;

            resetLightingSettings(path);
        }

    }
}
