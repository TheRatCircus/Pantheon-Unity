// WieldCommand.cs
// Jerome Martina

using Pantheon.Components.Entity;
using Pantheon.Core;
using UnityEngine;

namespace Pantheon.Commands.Actor
{
    using Actor = Components.Entity.Actor;

    public sealed class WieldCommand : ActorCommand
    {
        private Entity item;

        public WieldCommand(Entity entity, Entity wieldItem) : base(entity)
        {
            item = wieldItem;
            Cost = TurnScheduler.TurnTime;
        }

        public override CommandResult Execute()
        {
            if (!Entity.TryGetComponent(out Wield wield))
            {
                return CommandResult.Failed;
            }

            wield.TryWield(item, out Entity unwielded);

            if (Actor.PlayerControlled(Entity))
            {
                if (unwielded != null)
                    Locator.Log.Send(
                        $"You unwield {unwielded.ToSubjectString(false)}.",
                        Color.white);

                Locator.Log.Send($"You wield {item.ToSubjectString(false)}.",
                    Color.white);
            }
            
            return CommandResult.Succeeded;
        }
    }
}
