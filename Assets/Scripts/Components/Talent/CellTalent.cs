// CellTalent.cs
// Jerome Martina

using Pantheon.Commands;
using Pantheon.Utils;
using Pantheon.World;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Pantheon.Components.Talent
{
    using Entity = Pantheon.Entity;

    [Serializable]
    public sealed class CellTalent : TalentBehaviour
    {
        public int Accuracy { get; set; } = 50;
        public Damage[] Damages { get; set; }

        public override HashSet<Vector2Int> GetTargetedCells(Entity caster, Vector2Int target)
        {
            Line line = Bresenhams.GetLine(caster.Level, caster.Cell, target);
            return new HashSet<Vector2Int> { line.ElementAtOrLast(Range) };
        }

        public override CommandResult Invoke(Entity caster, Vector2Int target)
        {
            if (target == null)
                throw new NotImplementedException("Target cell needed.");

            Vector2Int affected = Bresenhams.GetLine(
                caster.Level, caster.Cell, target).ElementAtOrLast(Range);

            Entity enemy = caster.Level.ActorAt(affected);

            if (enemy == null)
            {
                Locator.Audio.Buffer(
                    Assets.Audio["SFX_Toss"],
                    affected.ToVector3());
                Locator.Log.Send(
                    $"{Strings.Subject(caster, true)}" +
                    $" {Verbs.Swing(caster)} at nothing.",
                    Color.grey);
                return CommandResult.Succeeded;
            }

            if (Accuracy < Random.Range(0, 101))
            {
                Locator.Log.Send(
                    Verbs.Miss(caster, enemy), Color.grey);
                return CommandResult.Succeeded;
            }

            Locator.Audio.Buffer(
                Assets.Audio["SFX_Punch"],
                affected.ToVector3());

            Hit hit = new Hit(Damages);
            Locator.Log.Send(Verbs.Hit(caster, enemy, hit), Color.white);
            enemy.TakeHit(caster, hit);
            return CommandResult.Succeeded;
        }
    }
}
