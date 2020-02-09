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
                Locator.Log.Send(
                        $"You aren't capable of wielding things.",
                        Color.yellow);
                return CommandResult.Failed;
            }

            if (!wield.TryWield(item, out Entity unwielded))
            {
                // Failure reason logged by component
                return CommandResult.Failed;
            }

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
