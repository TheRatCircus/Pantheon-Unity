// SaveWriterReader.cs
// Jerome Martina

using Pantheon.Core;
using Pantheon.ECS;
using Pantheon.Serialization.Binary.Surrogates;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Pantheon.SaveLoad
{
    public sealed class SaveWriterReader
    {
        SurrogateSelector selector;

        public SaveWriterReader(AssetLoader loader)
        {
            if (loader == null)
                throw new System.ArgumentNullException();

            selector = new SurrogateSelector();
            StreamingContext ctxt = new StreamingContext(StreamingContextStates.All);

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
        }

        public void WriteSave(Save save)
        {
            UnityEngine.Profiling.Profiler.BeginSample("Save Write");

            BinaryFormatter formatter = new BinaryFormatter();
            formatter.SurrogateSelector = selector;

            string path = Path.Combine(Application.persistentDataPath,
                $"{save.Name.ToLower()}.save");
            FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, save.Name);
            formatter.Serialize(stream, save);
            stream.Close();

            UnityEngine.Profiling.Profiler.EndSample();
            UnityEngine.Debug.Log($"Game successfully saved to {path}");
        }

        public Save ReadSave(string path)
        {
            UnityEngine.Profiling.Profiler.BeginSample("Save Read");

            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.SurrogateSelector = selector;

                FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                // Move stream to save object
                string name = formatter.Deserialize(stream) as string;
                Save save = formatter.Deserialize(stream) as Save;
                stream.Close();

                UnityEngine.Profiling.Profiler.EndSample();
                return save;
            }
            else
                throw new System.Exception($"Save file not found in {path}.");
        }
    }
}
