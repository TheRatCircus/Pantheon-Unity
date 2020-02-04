// AdjacentAttack.cs
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
    public sealed class CircularAttack : TalentBehaviour
    {
        public Damage[] Damages { get; set; }
        public int Radius { get; set; } = 1;
        public int Range { get; set; } // Greater than 0 creates a ring
        public int Accuracy { get; set; }
        public AudioClip HitSound { get; set; }

        public override HashSet<Cell> GetTargetedCells(Entity caster, Cell target)
        {
            // XXX: There must be a better way
            Cell casterCell = caster.Cell;
            Level level = caster.Level;
            HashSet<Cell> ret = new HashSet<Cell>();

            Circle.DrawCircle(casterCell.Position.x, casterCell.Position.y,
            Range, delegate (int x, int y)
            {
                if (level.TryGetCell(x, y, out Cell cell))
                    ret.Add(cell);
            });

            Circle.DrawCircle(casterCell.Position.x, casterCell.Position.y,
            Radius, delegate (int x, int y)
            {
                if (level.TryGetCell(x, y, out Cell cell))
                    ret.Add(cell);
            });

            // Get cells inside bounds
            HashSet<Cell> excluded = Floodfill.FillIf(
                level, casterCell, (Cell c) => !ret.Contains(c));

            // Add cells outside circles
            for (int y = -Radius; y <= Radius; y++)
            {
                for (int x = -Radius; x <= Radius; x++)
                {
                    int iX = casterCell.X + x, iY = casterCell.Y + y;
                    if (level.TryGetCell(iX, iY, out Cell c))
                    {
                        if (ret.Contains(c))
                            break;

                        excluded.Add(c);
                    }
                }

                for (int x = Radius; x > -Radius; x--)
                {
                    int iX = casterCell.X + x, iY = casterCell.Y + y;
                    if (level.TryGetCell(iX, iY, out Cell c))
                    {
                        if (ret.Contains(c))
                            break;

                        excluded.Add(c);
                    }
                }
            }

            // Fill between circles and nowhere else
            ret.AddMany(Locator.Player.VisibleCells.Where(
                (Cell c) => !excluded.Contains(c)));

            // Filter for visibility
            return ret.Where((Cell c) => c.Visible).ToHashSet();
        }

        public override CommandResult Invoke(Entity caster, Cell target)
        {
            if (target == null)
                throw new System.NotImplementedException(
                    "Target cell needed.");

            HashSet<Cell> affected = GetTargetedCells(caster, target);

            Locator.Audio.Buffer(
                HitSound,
                target.Position.ToVector3());

            foreach (Cell c in affected)
            {
                Entity enemy = c.Actor;
                if (enemy == null)
                    continue;

                if (Accuracy < Random.Range(0, 101))
                {
                    Locator.Log.Send(
                        Verbs.Miss(caster, enemy), Color.grey);
                    continue;
                }

                Hit hit = new Hit(Damages);
                Locator.Log.Send(Verbs.Hit(caster, enemy, hit), Color.white);
                enemy.TakeHit(caster, hit);
            }

            return CommandResult.Succeeded;
        }
    }
}