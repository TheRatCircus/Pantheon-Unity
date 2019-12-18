// BuildAssetBundles.cs
// Unity Technologies

using System.Diagnostics;
using System.IO;
using UnityEditor;

namespace PantheonEditor
{
    public static class BuildAssetBundles
    {
        [MenuItem("Assets/Build AssetBundles")]
        private static void BuildAllAssetBundles()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            string assetBundleDir = "Assets/AssetBundles";
            if (!Directory.Exists(assetBundleDir))
            {
                Directory.CreateDirectory(assetBundleDir);
            }
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
