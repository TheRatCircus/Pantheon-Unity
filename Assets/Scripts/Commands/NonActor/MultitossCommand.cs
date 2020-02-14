// MultitossCommand.cs
// Jerome Martina

using Pantheon.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActorComp = Pantheon.Components.Entity.Actor;

namespace Pantheon.Commands.NonActor
{
    [System.Serializable]
    public sealed class MultitossCommand : NonActorCommand
    {
        private Entity item;
        private int count;

        private static readonly float delay = .1f;

        public MultitossCommand(Entity entity, Entity item, int count) : base(entity)
        {
            this.item = item;
            this.count = count;
        }

        public override CommandResult Execute()
        {
            Line[] lines = FindTargets();
            if (lines == null)
                return CommandResult.Failed;
            else
            {
                Global.Instance.StartCoroutine(Fire(lines));
                return CommandResult.Succeeded;
            }
        }

        private IEnumerator Fire(Line[] lines)
        {
            Locator.Scheduler.Lock();
            for (int i = 0; i < count; i++)
            {
                GameObject tossFXObj = Object.Instantiate(
                    Assets.TossFXPrefab,
                    Entity.Cell.ToVector3(),
                    new Quaternion());
                LineProjectile proj = tossFXObj.GetComponent<LineProjectile>();
                proj.InitializeToss(Entity, item, lines[i]);
                proj.Fire();
                yield return new WaitForSeconds(delay);
            }
            Locator.Scheduler.Unlock();
        }

        private Line[] FindTargets()
        {
            Line[] ret = new Line[count];

            List<Entity> enemies = new List<Entity>();
            foreach (Vector2Int c in Locator.Player.VisibleCells)
            {
                Entity e = Level.ActorAt(c);
                if (e != null &&
                    e.Visible &&
                    e.TryGetComponent(out ActorComp actor) &&
                    Entity.GetComponent<ActorComp>().HostileTo(actor))
                {
                    enemies.Add(e);
                }
            }
            
            if (enemies.Count < 1)
                return null;

            for (int i = 0; i < count; i++)
            {
                ret[i] = Bresenhams.GetLine(
                    Entity.Level, Entity.Cell, enemies.ElementAtOrLast(i).Cell);
            }

            return ret;
        }
    }
}
