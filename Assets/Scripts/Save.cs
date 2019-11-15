// Save.cs
// Jerome Martina

using Pantheon.ECS;
using Pantheon.World;

namespace Pantheon.Core
{
    [System.Serializable]
    public sealed class Save
    {
        public string Name { get; private set; }
        public GameWorld World { get; private set; }
        public EntityManager Manager { get; private set; }

        public Save(string name, GameWorld world, EntityManager manager)
        {
            Name = name;
            //World = world;
            Manager = manager;
        }
    }
}
