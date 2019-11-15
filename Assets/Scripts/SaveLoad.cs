// SaveLoad.cs
// Jerome Martina

using Pantheon.Core;
using Pantheon.Utils;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Pantheon
{
    public static class SaveLoad
    {
        public static void Save(Save save)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.SurrogateSelector = Serialization.GetSurrogateSelector();

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
                formatter.SurrogateSelector = Serialization.GetSurrogateSelector();

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
