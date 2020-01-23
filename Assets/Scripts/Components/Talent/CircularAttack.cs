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
    public sealed class CircularAttack : TalentComponent
    {
        public Damage[] Damages { get; set; }
        public int Radius { get; set; } = 1;
        public int Range { get; set; } // Greater than 0 creates a toroid
        public AudioClip HitSound { get; set; }

        public override CommandResult Invoke(Entity caster, Cell target)
        {
            if (target == null)
                throw new System.NotImplementedException(
                    "Target cell needed.");

            HashSet<Cell> circleBounds = new HashSet<Cell>();
            Circle.DrawCircle(caster.Cell.Position.x, caster.Cell.Position.y,
            Range, (int x, int y) => circleBounds.Add(caster.Level.GetCell(x, y)));

            Circle.DrawCircle(caster.Cell.Position.x, caster.Cell.Position.y,
            Radius, (int x, int y) => circleBounds.Add(caster.Level.GetCell(x, y)));

            HashSet<Cell> circleArea = Floodfill.FillLevel(
                caster.Level, 
                caster.Level.Translate(caster.Cell, 0, Range + 1), 
                (Cell c) => !circleBounds.Contains(c));

            Locator.Audio.Buffer(
                HitSound,
                target.Position.ToVector3());

            foreach (Cell c in circleArea)
            {
                Entity enemy = c.Actor;
                if (enemy == null)
                    continue;

                Hit hit = new Hit(Damages);
                Locator.Log.Send(Verbs.Hit(caster, enemy, hit), Color.white);
                enemy.TakeHit(caster, hit);
            }
            foreach (Cell c in circleBounds)
            {
                Entity enemy = c.Actor;
                if (enemy == null)
                    continue;

                Hit hit = new Hit(Damages);
                Locator.Log.Send(Verbs.Hit(caster, enemy, hit), Color.white);
                enemy.TakeHit(caster, hit);
            }

            return CommandResult.Succeeded;
        }
    }
}