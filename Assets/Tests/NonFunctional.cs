using NUnit.Framework;
using UnityEngine;
using UnityEditor;


namespace SanityChecks
{
    public class NonFunctional
    {
        [Test]
        public void LaunchLightingWindow()
        {
            EditorApplication.ExecuteMenuItem("Window/Rendering/Lighting");
        }

        [Test]
        public void LaunchLightExplorerWindow()
        {
            EditorApplication.ExecuteMenuItem("Window/Rendering/Light Explorer");
        }

        [Test]
        public void CreateObjectAndMarkAsStatic()
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var newFlags = StaticEditorFlags.BatchingStatic | StaticEditorFlags.ContributeGI;
            GameObjectUtility.SetStaticEditorFlags(go, newFlags);
        }

        [Test]
        public void CleanLighting()
        {
            Lightmapping.Clear();
            Lightmapping.ClearDiskCache();
            Lightmapping.ClearLightingDataAsset();
        }

        [Test]
        public void CreateLightingSettingsAsset()
        {
            LightingSettings lightingSettings = null;
            if (!Lightmapping.TryGetLightingSettings(out lightingSettings))
                lightingSettings = new LightingSettings();
            Lightmapping.lightingSettings = lightingSettings;
        }

        [Test]
        public void ChangeToEnlighten()
        {
            LightingSettings lightingSettings = new LightingSettings();
            if (!Lightmapping.TryGetLightingSettings(out lightingSettings))
                lightingSettings = new LightingSettings();
            Lightmapping.lightingSettings = lightingSettings;
            Lightmapping.lightingSettings.lightmapper = LightingSettings.Lightmapper.Enlighten;

            Assert.IsTrue(Lightmapping.lightingSettings.lightmapper == LightingSettings.Lightmapper.Enlighten);
        }

        [Test]
        public void ChangeToCPU_PLM()
        {
            LightingSettings lightingSettings = new LightingSettings();
            if (!Lightmapping.TryGetLightingSettings(out lightingSettings))
                lightingSettings = new LightingSettings();
            Lightmapping.lightingSettings = lightingSettings;
            Lightmapping.lightingSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveCPU;

            Assert.IsTrue(Lightmapping.lightingSettings.lightmapper == LightingSettings.Lightmapper.ProgressiveCPU);
        }

        [Test]
        public void ChangeToGPU_PLM()
        {
            LightingSettings lightingSettings = new LightingSettings();
            if (!Lightmapping.TryGetLightingSettings(out lightingSettings))
                lightingSettings = new LightingSettings();
            Lightmapping.lightingSettings = lightingSettings;
            Lightmapping.lightingSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveGPU;

            Assert.IsTrue(Lightmapping.lightingSettings.lightmapper == LightingSettings.Lightmapper.ProgressiveGPU);
        }
    }
}
