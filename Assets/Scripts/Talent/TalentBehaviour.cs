// TalentBehaviour.cs
// Jerome Martina

using Pantheon.Commands;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon
{
    [System.Serializable]
    public abstract class TalentBehaviour
    {
        public int Range { get; set; } = 1;

        public abstract CommandResult Invoke(
            Entity caster, 
            Entity evoked, 
            Vector2Int target);
        public abstract HashSet<Vector2Int> GetTargetedCells(
            Entity caster, 
            Entity evoked, 
            Vector2Int target);
    }
}
