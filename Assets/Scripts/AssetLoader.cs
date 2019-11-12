// AssetLoader.cs
// Jerome Martina

using System.IO;
using UnityEngine;

namespace Pantheon
{
    public sealed class AssetLoader
    {
        private AssetBundle bundle;

        public AssetLoader()
        {
            bundle = AssetBundle.LoadFromFile(Path.Combine(
                Application.streamingAssetsPath, "pantheon"));
            System.Diagnostics.Debug.Assert(bundle != null);
        }

        public T Load<T>(string name) where T : Object
        {
            if (!bundle.Contains(name))
                throw new System.ArgumentException(
                    $"{name} not found in bundle {bundle.name}.");

            T obj = bundle.LoadAsset<T>(name);
            return obj;
        }

        public void Unload(bool unloadAllLoadedObjects)
        {
            bundle.Unload(unloadAllLoadedObjects);
        }
    }
}
