// CreateAssetBundles.cs
// Unity Technologies

using UnityEditor;
using System.IO;

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
    }
}
