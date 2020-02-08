// TalentBehaviour.cs
// Jerome Martina

using Pantheon.Commands;
using Pantheon.World;
using System.Collections.Generic;

namespace Pantheon
{
    [System.Serializable]
    public abstract class TalentBehaviour
    {
        public int Range { get; set; } = 1;

        public abstract CommandResult Invoke(Entity caster, Cell target);
        public abstract HashSet<Cell> GetTargetedCells(Entity caster, Cell target);
    }
}
