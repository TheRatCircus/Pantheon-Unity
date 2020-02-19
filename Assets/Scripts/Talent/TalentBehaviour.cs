// TalentBehaviour.cs
// Jerome Martina

using Pantheon.Commands;
using Pantheon.World;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon
{
    [System.Serializable]
    public abstract class TalentBehaviour
    {
        public int Range { get; set; } = 1;

        public IEntityTalentEffect CasterEffects { get; set; }
        public ICellTalentEffect CasterCellEffects { get; set; }

        public abstract CommandResult Invoke(
            Entity caster, Vector2Int target);
        public abstract HashSet<Vector2Int> GetTargetedCells(
            Entity caster, Vector2Int target);
    }

    public interface IEntityTalentEffect
    {
        void Affect(Entity source, Entity target);
    }

    public interface ICellTalentEffect
    {
        void Affect(Entity source, Level level, Vector2Int cell);
        Vector2Int[] GetAffectedCells(Entity source, Level level, Vector2Int cell);
    }
}
