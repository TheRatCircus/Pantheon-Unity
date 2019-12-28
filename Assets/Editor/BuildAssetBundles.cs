// BuildAssetBundles.cs
// Unity Technologies

using System.Diagnostics;
using UnityEditor;

namespace PantheonEditor
{
    public static class BuildAssetBundles
    {
        [MenuItem("Assets/Build AssetBundles")]
        private static void BuildAllAssetBundles()
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
