// RingTalent.cs
// Jerome Martina

using Pantheon.Commands;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon
{
    public class RingTalent : TalentBehaviour
    {
        public Damage[] Damages { get; set; }
        public int Radius { get; set; } = 3;
        // Greater Range than 0 creates a ring
        public int Accuracy { get; set; }
        public AudioClip HitSound { get; set; }

        public override HashSet<Vector2Int> GetTargetedCells(
            Entity caster, Vector2Int target)
        {
            throw new System.NotImplementedException();
        }

        public override CommandResult Invoke(
            Entity caster, Vector2Int target)
        {
            throw new System.NotImplementedException();
        }
    }
}
