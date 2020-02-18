// RingTalent.cs
// Jerome Martina

using Pantheon.Commands;
using Pantheon.Utils;
using Pantheon.World;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pantheon.Components.Talent
{
    using Entity = Pantheon.Entity;

    [System.Serializable]
    public sealed class RingTalent : TalentBehaviour
    {
        public Damage[] Damages { get; set; }
        public int Radius { get; set; } = 3;
        // Greater Range than 0 creates a ring
        public int Accuracy { get; set; }
        public AudioClip HitSound { get; set; }

        public override HashSet<Vector2Int> GetTargetedCells(
            Entity caster, Entity evoked, Vector2Int target)
        {
            throw new System.NotImplementedException();
        }

        public override CommandResult Invoke(
            Entity caster, Entity evoked, Vector2Int target)
        {
            throw new System.NotImplementedException();
        }
    }
}
