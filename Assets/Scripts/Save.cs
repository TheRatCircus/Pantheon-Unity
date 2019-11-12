// Save.cs
// Jerome Martina

using Pantheon.World;

namespace Pantheon.Core
{
    [System.Serializable]
    public sealed class Save
    {
        public GameWorld World { get; set; }
    }
}
