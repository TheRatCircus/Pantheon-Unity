// CellTalent.cs
// Jerome Martina

using Pantheon.Commands;
using Pantheon.Components.Entity;
using Pantheon.Utils;
using Pantheon.World;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Pantheon
{
    public class CellTalent : TalentBehaviour
    {
        public TalentFlavour Flavour { get; set; }
        public int Accuracy { get; set; } = 50;
        public Damage[] Damages { get; set; }
        public int Radius { get; set; } = 1;
        public AudioClip HitSound { get; set; }
        public AudioClip MissSound { get; set; }

        public ICellTalentEffect[] Effects { get; set; }

        public override HashSet<Vector2Int> GetTargetedCells(
            Entity caster, Vector2Int target)
        {
            Line line = Bresenhams.GetLine(caster.Level, caster.Cell, target);
            return new HashSet<Vector2Int>(
                caster.Level.GetSquare(line.ElementAtOrLast(Range), Radius));
        }

        public override CommandResult Invoke(Entity caster, Vector2Int target)
        {
            if (target == Level.NullCell)
            {
                if (Actor.PlayerControlled(caster))
                    Locator.Log.Send($"You can't cast there.",
                        Color.grey);
                else
                    UnityEngine.Debug.LogWarning(
                        $"{caster} tried to cast a null cell.");
                return CommandResult.Failed;
            }

            List<Vector2Int> affected = caster.Level.GetSquare(
                Bresenhams.GetLine(
                    caster.Level,
                    caster.Cell,
                    target).ElementAtOrLast(Range),
                Radius);

            bool enemyPresent = false;

            foreach (Vector2Int cell in affected)
            {
                Entity enemy = caster.Level.ActorAt(cell);

                if (enemy == null)
                    continue;

                enemyPresent = true;

                if (Accuracy < Random.Range(0, 101))
                {
                    Locator.Log.Send(
                        $"{Strings.Subject(caster, true)} " +
                        $"{Verbs.Miss(caster, enemy)} " +
                        $"{Strings.Subject(enemy)}." ,
                        Color.grey);
                    continue;
                }

                Hit hit = new Hit(Damages);
                Locator.Log.Send(Verbs.Hit(caster, enemy, hit), Color.white);
                enemy.TakeHit(caster, hit);
            }

            Vector2Int soundPos = affected.ElementAtOrDefault(affected.Count / 2);
            if (soundPos == Vector2Int.zero)
                soundPos = caster.Cell;

            if (enemyPresent)
            {
                Locator.Audio.Buffer(
                    HitSound,
                    affected.ElementAtOrDefault(affected.Count / 2).ToVector3());
            }
            else
            {
                Locator.Audio.Buffer(
                    MissSound,
                    affected.ElementAtOrDefault(affected.Count / 2).ToVector3());
                Locator.Log.Send(
                    $"{Strings.Subject(caster, true)} " +
                    $"{Verbs.TalentFlavourVerb(caster, Flavour)} nothing.",
                    Color.grey);
            }

            return CommandResult.Succeeded;
        }
    }
}
