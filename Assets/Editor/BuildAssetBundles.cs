// BuildAssetBundles.cs
// Unity Technologies

using System.Diagnostics;
using UnityEditor;

namespace PantheonEditor
{
    public static class BuildAssetBundles
    {
        [MenuItem("Assets/Build AssetBundles")]
#pragma warning disable IDE0051 // Remove unused private members
        private static void BuildAllAssetBundles()
#pragma warning restore IDE0051
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            BuildPipeline.BuildAssetBundles(
                "Assets/StreamingAssets",
                BuildAssetBundleOptions.None,
                BuildTarget.StandaloneWindows64);
            stopwatch.Stop();
            UnityEngine.Debug.Log(
                $"Finished building AssetBundles in {stopwatch.ElapsedMilliseconds} ms.");
        }
    }
}
