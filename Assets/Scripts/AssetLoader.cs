// AssetLoader.cs
// Jerome Martina

using System;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Pantheon
{
    public sealed class AssetLoader : MonoBehaviour
    {
        private AssetBundle bundle;

        private void Awake()
        {
            bundle = AssetBundle.LoadFromFile(Path.Combine(
                Application.streamingAssetsPath, "pantheon"));
            System.Diagnostics.Debug.Assert(bundle != null);
        }

        public T Load<T>(string name) where T : Object
        {
            UnityEngine.Profiling.Profiler.BeginSample("AssetLoader.Load()");
            if (!bundle.Contains(name))
                throw new ArgumentException(
                    $"{name} not found in bundle {bundle.name}.");

            T obj = bundle.LoadAsset<T>(name);
            UnityEngine.Profiling.Profiler.EndSample();
            return obj;
        }

        public T TryLoad<T>(string name) where T : Object
        {
            if (!bundle.Contains(name))
                return null;

            T obj = bundle.LoadAsset<T>(name);
            return obj;
        }
    }
}
