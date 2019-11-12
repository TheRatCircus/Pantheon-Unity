// BuilderStep.cs
// Jerome Martina

using Pantheon.ECS;
using Pantheon.World;

namespace Pantheon.Gen
{
    /// <summary>
    /// A stage in a level builder.
    /// </summary>
    [System.Serializable]
    public abstract class BuilderStep
    {
        public abstract void Run(Level level, EntityFactory factory);
    }
}
