// BuilderStep.cs
// Jerome Martina

using Pantheon.World;
using UnityEngine;

namespace Pantheon.Gen
{
    /// <summary>
    /// A stage in a level builder.
    /// </summary>
    public abstract class BuilderStep
    {
        public abstract void Run(Level level);
    }
}
