using UnityEditor;
using UnityEditor.Build.Reporting;

// Headless WebGL build, called by deploy.sh: Unity.exe -batchmode -executeMethod WebBuilder.Build
public static class WebBuilder
{
    [MenuItem("Build/WebGL to WebBuild")]
    public static void Build()
    {
        // Fill: game fills its frame and takes keyboard focus with the page (Assets/WebGLTemplates/Fill).
        PlayerSettings.WebGL.template = "PROJECT:Fill";
        // Lets the compressed build run on hosts that don't send Content-Encoding headers (GitHub Pages).
        PlayerSettings.WebGL.decompressionFallback = true;

        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes),
            locationPathName = "WebBuild",
            target = BuildTarget.WebGL,
        });
        if (report.summary.result != BuildResult.Succeeded) EditorApplication.Exit(1);
    }
}
