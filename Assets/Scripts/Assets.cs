// Assets.cs
// Jerome Martina

using System.IO;
using UnityEngine;

namespace Pantheon
{
    public static class Assets
    {
        public static T Load<T>(string name) where T : Object
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(Path.Combine(
                Application.streamingAssetsPath, "pantheon"));
            System.Diagnostics.Debug.Assert(bundle != null);
            T obj = bundle.LoadAsset<T>(name);
            return obj;
        }
    }
}
