// Save.cs
// Jerome Martina

using Pantheon.World;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Pantheon.SaveLoad
{
    [Serializable]
    public class Save
    {
        public string Name { get; set; }
        public GameWorld World { get; private set; }
        public Entity Player { get; private set; }

        public Save(string name, GameWorld world, Entity player)
        {
            Name = name;
            //World = world;
            Player = player;
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
