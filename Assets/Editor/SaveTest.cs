// SaveTest.cs
// Jerome Martina

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEditor;
using Pantheon;
using Pantheon.World;
using Pantheon.Serialization.Binary.Surrogates;
using Pantheon.Core;
using Pantheon.Content;
using Pantheon.Gen;

namespace PantheonEditor
{
    internal static class SaveTest
    {
        [MenuItem("Tools/Pantheon/Test Save Size")]
        private static void TestSaveSize()
        {
            AssetBundle.UnloadAllAssetBundles(true);
            Assets.LoadAssets();
            GameWorld world = new GameWorld();
            world.NewLayer(-2);
            world.Layers.TryGetValue(-2, out Layer surface);
            Level level = surface.RequestLevel(Vector2Int.zero);

            SurrogateSelector selector = new SurrogateSelector();
            StreamingContext ctxt = new StreamingContext(StreamingContextStates.All);

            AssetLoader loader = new AssetLoader();

            selector.AddSurrogate(typeof(Vector3Int), ctxt,
                new Vector3IntSurrogate());
            selector.AddSurrogate(typeof(Vector2Int), ctxt,
                new Vector2IntSurrogate());
            selector.AddSurrogate(typeof(ScriptableObject), ctxt,
                new EntityTemplateSurrogate(loader));
            selector.AddSurrogate(typeof(TerrainDefinition), ctxt,
                new TerrainDefSurrogate(loader));
            selector.AddSurrogate(typeof(Sprite), ctxt,
                new SpriteSurrogate(loader));
            selector.AddSurrogate(typeof(SpeciesDefinition), ctxt,
                new SpeciesDefSurrogate(loader));
            selector.AddSurrogate(typeof(EntityTemplate), ctxt,
                new EntityTemplateSurrogate(loader));
            selector.AddSurrogate(typeof(BuilderPlan), ctxt,
                new BuilderPlanSurrogate(loader));
            selector.AddSurrogate(typeof(GameObject), ctxt,
                new PrefabSurrogate(loader));
            selector.AddSurrogate(typeof(AudioClip), ctxt,
                new AudioClipSurrogate(loader));

            BinaryFormatter formatter = new BinaryFormatter
            {
                SurrogateSelector = selector
            };

            string path = Path.Combine(Application.persistentDataPath,
                $"test_save.save");
            FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, world);
            stream.Close();
            FileInfo info = new FileInfo(path);
            Debug.Log($"Size of save: {info.Length}");
        }
    }
}
