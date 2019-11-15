// SaveLoad.cs
// Jerome Martina

using Pantheon.Core;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Pantheon
{
    public static class SaveLoad
    {
        public static void Save(Save save)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            SurrogateSelector selector = new SurrogateSelector();

            Vector3IntSerializationSurrogate vector3ISS = 
                new Vector3IntSerializationSurrogate();
            Vector2IntSerializationSurrogate vector2ISS =
                new Vector2IntSerializationSurrogate();
            selector.AddSurrogate(typeof(Vector3Int),
                new StreamingContext(StreamingContextStates.All), vector3ISS);
            selector.AddSurrogate(typeof(Vector2Int),
                new StreamingContext(StreamingContextStates.All), vector2ISS);

            formatter.SurrogateSelector = selector;

            string path = Path.Combine(Application.persistentDataPath,
                $"{save.Name.ToLower()}.save");
            FileStream stream = new FileStream(path, FileMode.Create);
            formatter.Serialize(stream, save);
            UnityEngine.Debug.Log($"Game successfully saved to {path}");
        }

        public static Save Load(string path)
        {
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);
                Save save = formatter.Deserialize(stream) as Save;
                stream.Close();
                return save;
            }
            else
            {
                throw new System.Exception($"Save file not found in {path}.");
            }
        }
    }
}
