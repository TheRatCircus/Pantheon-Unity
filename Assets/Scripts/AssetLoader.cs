// AssetLoader.cs
// Jerome Martina

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Pantheon
{
    [Serializable]
    public sealed class AssetLoader
    {
        [NonSerialized] private AssetBundle bundle;

        //public Dictionary<string, Object> Assets { get; }
        //    = new Dictionary<string, Object>();

        public AssetLoader()
        {
            bundle = AssetBundle.LoadFromFile(Path.Combine(
                Application.streamingAssetsPath, "pantheon"));
            System.Diagnostics.Debug.Assert(bundle != null);
        }

        public Object GetAsset(string name)
        {
            return Load<Object>(name);
        }

        public T Load<T>(string name) where T : Object
        {
            if (!bundle.Contains(name))
                throw new ArgumentException(
                    $"{name} not found in bundle {bundle.name}.");

            T obj = bundle.LoadAsset<T>(name);
            //Assets.Add(obj.name, obj);
            return obj;
        }
    }

    public static class Assets
    {
        public static T Load<T>(string name) where T : Object
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(Path.Combine(
                Application.streamingAssetsPath, "pantheon"));
            System.Diagnostics.Debug.Assert(bundle != null);

            if (!bundle.Contains(name))
                throw new ArgumentException(
                    $"{name} not found in bundle {bundle.name}.");

            T obj = bundle.LoadAsset<T>(name);
            return obj;
        }
    }
}
