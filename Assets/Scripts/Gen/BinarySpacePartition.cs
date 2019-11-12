// BinarySpacePartition.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon.ECS;
using Pantheon.World;

namespace Pantheon.Gen
{
    [System.Serializable]
    public sealed class BinarySpacePartition : BuilderStep
    {
        public override void Run(Level level, EntityFactory factory)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return $"Running binary space partition on level.";
        }
    }
}
