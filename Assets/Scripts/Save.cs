// Save.cs
// Jerome Martina

using Pantheon.Gen;
using Pantheon.World;

namespace Pantheon.Core
{
    [System.Serializable]
    public sealed class Save
    {
        public string Name { get; private set; }
        public GameWorld World { get; private set; }
        public LevelGenerator Generator { get; private set; }
    }
}
