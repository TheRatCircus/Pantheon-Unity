// AdjacentAttack.cs
// Jerome Martina

using Pantheon.Commands;
using Pantheon.Utils;
using Pantheon.World;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.Components.Talent
{
    using Entity = Pantheon.Entity;

    [System.Serializable]
    public sealed class AdjacentAttack : TalentComponent
    {
        public Damage[] Damages { get; set; }

        public override HashSet<Cell> GetTargetedCells(Entity caster, Cell target)
        {
            Line line = Bresenhams.GetLine(caster.Level, caster.Cell, target);
            return new HashSet<Cell> { line.ElementAtOrLast(1) };
        }

        public override CommandResult Invoke(Entity caster, Cell target)
        {
            if (target == null)
                throw new System.NotImplementedException(
                    "Target cell needed.");

            Entity enemy = target.Actor;

            if (enemy == null)
            {
                Locator.Audio.Buffer(
                    Assets.Audio["SFX_Toss"],
                    target.Position.ToVector3());
                Locator.Log.Send(
                    $"{caster.ToSubjectString(true)}" +
                    $" {Verbs.Swing(caster)} at nothing.",
                    Color.grey);
                return CommandResult.Succeeded;
            }

            Locator.Audio.Buffer(
                Assets.Audio["SFX_Punch"],
                target.Position.ToVector3());

            Hit hit = new Hit(Damages);
            Locator.Log.Send(Verbs.Hit(caster, enemy, hit), Color.white);
            enemy.TakeHit(caster, hit);
            return CommandResult.Succeeded;
        }
    }
}
