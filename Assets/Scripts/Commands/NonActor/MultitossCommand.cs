// MultitossCommand.cs
// Jerome Martina

using Pantheon.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActorComp = Pantheon.Components.Actor;

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
                GlobalVars.Inst.StartCoroutine(Fire(lines));
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
                    Entity.Position.ToVector3(),
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
            List<Entity> enemies = Entity.Level.FindBySpiral(Entity.Position, 11,
                delegate (Entity e)
                {
                    if (e == null)
                        return false;

                    if (!e.Visible)
                        return false;

                    if (!e.TryGetComponent(out ActorComp actor))
                        return false;

                    if (!Entity.GetComponent<ActorComp>().HostileTo(actor))
                        return false;

                    return true;
                });

            if (enemies.Count < 1)
                return null;

            for (int i = 0; i < count; i++)
            {
                ret[i] = Bresenhams.GetLine(
                    Entity.Position, enemies.ElementAtOrLast(i).Position);
            }

            return ret;
        }
    }
}
