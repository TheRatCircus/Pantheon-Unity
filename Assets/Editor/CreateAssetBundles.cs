// CreateAssetBundles.cs
// Unity Technologies

using UnityEditor;
using System.IO;

namespace PantheonEditor
{
    public static class CreateAssetBundles
    {
        [MenuItem("Assets/Build AssetBundles")]
        private static void BuildAllAssetBundles()
        {
            string assetBundleDir = "Assets/AssetBundles";
            if (!Directory.Exists(assetBundleDir))
            {
                Directory.CreateDirectory(assetBundleDir);
            }
            BuildPipeline.BuildAssetBundles(
                "Assets/StreamingAssets",
                BuildAssetBundleOptions.None,
                BuildTarget.StandaloneWindows64);
            UnityEngine.Debug.Log("Finished building AssetBundles.");
        }
    }
}
