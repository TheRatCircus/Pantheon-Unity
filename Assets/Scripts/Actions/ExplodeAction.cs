// ExplodeAction.cs
// Jerome Martina

using Pantheon.Actors;
using Pantheon.Core;
using Pantheon.World;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.Actions
{
    [Serializable]
    public sealed class ExplodeAction : BaseAction
    {
        [SerializeField] private GameObject fxPrefab = null;

        public Cell Origin { get; set; }
        [SerializeField] private int radius = -1;
        [SerializeField] private int minDamage = -1;
        [SerializeField] private int maxDamage = -1;

        public ExplodeAction(Actor actor, int radius)
            : base(actor)
        {
            this.radius = radius;
        }

        public override int DoAction()
        {
            List<Cell> cells = new List<Cell>();
            Level level = Game.instance.activeLevel;

            int x1 = Origin.Position.x - (radius - 1);
            int x2 = Origin.Position.x + (radius - 1);
            int y1 = Origin.Position.y - (radius - 1);
            int y2 = Origin.Position.y + (radius - 1);

            for (int x = x1; x <= x2; x++)
                for (int y = y1; y <= y2; y++)
                {
                    Cell cell = level.GetCell(new Vector2Int(x, y));
                    if (cell.Actor != null)
                    {
                        Hit hit = new Hit(minDamage, maxDamage);
                        GameLog.Send($"{Utils.Strings.GetSubject(cell.Actor, true)} " +
                            $"{(cell.Actor is Player ? "are" : "is")} " +
                            $"caught in the blast, and takes {hit.Damage} damage!");
                        
                        cell.Actor.TakeHit(hit, Actor);
                    }
                }

            UnityEngine.Object.Destroy(UnityEngine.Object.Instantiate(fxPrefab,
                Utils.Helpers.V2IToV3(Origin.Position),
                new Quaternion()) as GameObject, 10);
            return -1;
        }

        public override int DoAction(OnConfirm onConfirm)
            => throw new NotImplementedException();

        public override string ToString()
            => $"{Actor.ActorName} caused a {radius}-cell-radius explosion at {Origin.Position}.";
    }
}