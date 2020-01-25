// BuildAssetBundles.cs
// Unity Technologies

using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace PantheonEditor
{
    public static class AssetBundleEditor
    {
#pragma warning disable IDE0051
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

        [MenuItem("Tools/Unload AssetBundles")]
        private static void ForceUnload()
        {
            AssetBundle.UnloadAllAssetBundles(true);
        }
#pragma warning restore 
    }
}
