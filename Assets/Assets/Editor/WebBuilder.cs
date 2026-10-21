using UnityEditor;
using UnityEditor.Build.Reporting;

// Headless WebGL build, called by deploy.sh: Unity.exe -batchmode -executeMethod WebBuilder.Build
public static class WebBuilder
{
    [MenuItem("Build/WebGL to WebBuild")]
    public static void Build()
    {
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes),
            locationPathName = "WebBuild",
            target = BuildTarget.WebGL,
        });
        if (report.summary.result != BuildResult.Succeeded) EditorApplication.Exit(1);
    }
}
