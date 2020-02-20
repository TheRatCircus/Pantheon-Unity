// Save.cs
// Jerome Martina

using Pantheon.Content;
using Pantheon.Serialization.Binary.Surrogates;
using Pantheon.World;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Pantheon.SaveLoad
{
    [Serializable]
    public sealed class Save
    {
        public string Name { get; set; }
        public GameWorld World { get; private set; }
        public Entity Player { get; private set; }

        public Save(string name, GameWorld world, Entity player)
        {
            Name = name;
            World = world;
            Player = player;
        }

        public static void WriteSave(Save save)
        {
            UnityEngine.Profiling.Profiler.BeginSample("Save Write");
            Stopwatch stopwatch = Stopwatch.StartNew();

            SurrogateSelector selector = new SurrogateSelector();
            StreamingContext ctxt = new StreamingContext(StreamingContextStates.All);

            selector.AddSurrogate(typeof(Vector3Int), ctxt,
                new Vector3IntSurrogate());
            selector.AddSurrogate(typeof(Vector2Int), ctxt,
                new Vector2IntSurrogate());
            selector.AddSurrogate(typeof(EntityTemplate), ctxt,
                new EntityTemplateSurrogate());
            selector.AddSurrogate(typeof(TerrainDefinition), ctxt,
                new TerrainDefSurrogate());
            selector.AddSurrogate(typeof(Sprite), ctxt,
                new SpriteSurrogate());
            selector.AddSurrogate(typeof(SpeciesDefinition), ctxt,
                new SpeciesDefSurrogate());
            selector.AddSurrogate(typeof(GameObject), ctxt,
                new PrefabSurrogate());
            selector.AddSurrogate(typeof(AudioClip), ctxt,
                new AudioClipSurrogate());
            selector.AddSurrogate(typeof(Tile), ctxt,
                new TileSurrogate());
            selector.AddSurrogate(typeof(AIDefinition), ctxt,
                new AIDefinitionSurrogate());

            BinaryFormatter formatter = new BinaryFormatter
            {
                SurrogateSelector = selector
            };

            string path = Path.Combine(Application.persistentDataPath,
                $"{save.Name.ToLower()}.save");
            FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, save.Name);
            formatter.Serialize(stream, save);
            stream.Close();

            UnityEngine.Profiling.Profiler.EndSample();
            stopwatch.Stop();
            UnityEngine.Debug.Log(
                $"Game successfully saved to {path}, " +
                $"took {stopwatch.ElapsedMilliseconds} ms.");
        }

        public static Save ReadSave(string path)
        {
            UnityEngine.Profiling.Profiler.BeginSample("Save Read");

            SurrogateSelector selector = new SurrogateSelector();
            StreamingContext ctxt = new StreamingContext(StreamingContextStates.All);

            selector.AddSurrogate(typeof(Vector3Int), ctxt,
                new Vector3IntSurrogate());
            selector.AddSurrogate(typeof(Vector2Int), ctxt,
                new Vector2IntSurrogate());
            selector.AddSurrogate(typeof(EntityTemplate), ctxt,
                new EntityTemplateSurrogate());
            selector.AddSurrogate(typeof(TerrainDefinition), ctxt,
                new TerrainDefSurrogate());
            selector.AddSurrogate(typeof(Sprite), ctxt,
                new SpriteSurrogate());
            selector.AddSurrogate(typeof(SpeciesDefinition), ctxt,
                new SpeciesDefSurrogate());
            selector.AddSurrogate(typeof(GameObject), ctxt,
                new PrefabSurrogate());
            selector.AddSurrogate(typeof(AudioClip), ctxt,
                new AudioClipSurrogate());
            selector.AddSurrogate(typeof(Tile), ctxt,
                new TileSurrogate());
            selector.AddSurrogate(typeof(AIDefinition), ctxt,
                new AIDefinitionSurrogate());

            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter
                {
                    SurrogateSelector = selector
                };

                FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                // Move stream to save object
                string name = formatter.Deserialize(stream) as string;
                Save save = formatter.Deserialize(stream) as Save;
                stream.Close();

                UnityEngine.Profiling.Profiler.EndSample();
                return save;
            }
            else
                throw new Exception($"Save file not found in {path}.");
        }

        public static string ReadSaveName(string path)
        {
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                string name = formatter.Deserialize(stream) as string;
                stream.Close();
                return name;
            }
            else
                throw new ArgumentException($"Save file not found in {path}.");
        }
    }
}
