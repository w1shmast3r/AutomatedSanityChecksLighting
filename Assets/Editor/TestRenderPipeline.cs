using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class TestRenderPipeline : RenderPipeline
{
    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    { }
}

// DEFAULT WORKING

public class TestRenderPipelineAsset : RenderPipelineAsset
{
    protected override RenderPipeline CreatePipeline()
    {
        return new TestRenderPipeline();
    }
}

[LightingExplorerExtensionAttribute(typeof(TestRenderPipelineAsset))]
public class TestExplorerExtension : DefaultLightingExplorerExtension
{
    public override void OnEnable()
    {
        Debug.Log("OnEnableCalled");
    }

    public override void OnDisable()
    {
        Debug.Log("OnDisableCalled");
    }

    public override LightingExplorerTab[] GetContentTabs()
    {
        return new[]
        {
            new LightingExplorerTab("MyLightTableCustomName", GetLights, GetLightColumns)
        };
    }
}

public class TestTwoRenderPipelineAsset : RenderPipelineAsset
{
    protected override RenderPipeline CreatePipeline()
    {
        return new TestRenderPipeline();
    }
}

[LightingExplorerExtensionAttribute(typeof(TestTwoRenderPipelineAsset))]
public class TestTwoExplorerExtension : ILightingExplorerExtension
{
    public void OnEnable()
    {
        Debug.Log("OnEnableTwoCalled");
    }

    public void OnDisable()
    {
        Debug.Log("OnDisableTwoCalled");
    }

    public LightingExplorerTab[] GetContentTabs()
    {
        return new[]
        {
            new LightingExplorerTab("My Table", GetObjects, GetColumns)
        };
    }

    UnityEngine.Object[] GetObjects()
    {
        return UnityEngine.Object.FindObjectsOfType<Light>();
    }

    protected virtual LightingExplorerTableColumn[] GetColumns()
    {
        return new[]
        {
            new LightingExplorerTableColumn(LightingExplorerTableColumn.DataType.Name, new GUIContent("MyColumnCustomName"), null, 200), // 0: Name
        };
    }
}

// NULL COLUMNS

public class NullColumnsTestRenderPipelineAsset : RenderPipelineAsset
{
    protected override RenderPipeline CreatePipeline()
    {
        return new TestRenderPipeline();
    }
}

[LightingExplorerExtensionAttribute(typeof(NullColumnsTestRenderPipelineAsset))]
public class NullColumnsTestExplorerExtension : DefaultLightingExplorerExtension
{
    protected override LightingExplorerTableColumn[] GetLightColumns()
    {
        return null;
    }
}

// NULL CONTENT

public class NullContentTestRenderPipelineAsset : RenderPipelineAsset
{
    protected override RenderPipeline CreatePipeline()
    {
        return new TestRenderPipeline();
    }
}

[LightingExplorerExtensionAttribute(typeof(NullContentTestRenderPipelineAsset))]
public class NullContentTestExplorerExtension : DefaultLightingExplorerExtension
{
    protected override UnityEngine.Object[] GetLights()
    {
        return null;
    }
}

// NULL TABS

public class NullTabsTestRenderPipelineAsset : RenderPipelineAsset
{
    protected override RenderPipeline CreatePipeline()
    {
        return new TestRenderPipeline();
    }
}

[LightingExplorerExtensionAttribute(typeof(NullTabsTestRenderPipelineAsset))]
public class NullTabsTestExplorerExtension : DefaultLightingExplorerExtension
{
    public override LightingExplorerTab[] GetContentTabs()
    {
        return null;
    }
}