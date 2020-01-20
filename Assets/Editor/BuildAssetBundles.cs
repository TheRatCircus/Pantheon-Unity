// BuildAssetBundles.cs
// Unity Technologies

using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace PantheonEditor
{
    public static class BuildAssetBundles
    {
        [MenuItem("Tools/Build AssetBundles")]
#pragma warning disable IDE0051 // Remove unused private members
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

        [MenuItem("Tools/Unload AssetBundles")]
        private static void UnloadAllAssetBundles()
        {
            AssetBundle.UnloadAllAssetBundles(true);
        }
#pragma warning restore IDE0051 // Remove unused private members
    }
}
