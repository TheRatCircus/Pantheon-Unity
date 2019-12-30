// MultitossCommand.cs
// Jerome Martina

using Pantheon.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ActorComp = Pantheon.Components.Actor;

namespace Pantheon.Commands.NonActor
{
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
            PrefabProvider.Inst.StartCoroutine(Fire());
            return CommandResult.Succeeded;
        }

        private IEnumerator Fire()
        {
            Line[] lines = FindTargets();
            Locator.Scheduler.Lock();
            for (int i = 0; i < count; i++)
            {
                GameObject tossFXObj = Object.Instantiate(
                    PrefabProvider.TossFXPrefab,
                    Entity.Cell.Position.ToVector3(),
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
            List<Entity> enemies = Entity.Level.FindBySpiral(Entity.Cell, 11,
                delegate (Entity e)
                {
                    if (e == null)
                        return false;

                    if (!e.TryGetComponent(out ActorComp actor))
                        return false;

                    if (!Entity.GetComponent<ActorComp>().HostileTo(actor))
                        return false;

                    return true;
                });

            for (int i = 0; i < count; i++)
            {
                ret[i] = Bresenhams.GetLine(
                    Entity.Level, Entity.Cell, enemies.ElementAtOrLast(i).Cell);
            }

            return ret;
        }
    }
}
