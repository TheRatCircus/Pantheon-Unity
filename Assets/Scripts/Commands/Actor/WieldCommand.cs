// WieldCommand.cs
// Jerome Martina

using Pantheon.Components;
using Pantheon.Core;
using UnityEngine;

namespace Pantheon.Commands.Actor
{
    public sealed class WieldCommand : ActorCommand
    {
        private Entity item;

        public WieldCommand(Entity entity, Entity wieldItem) : base(entity)
        {
            item = wieldItem;
        }

        public override CommandResult Execute(out int cost)
        {
            if (!Entity.TryGetComponent(out Wield wield))
            {
                cost = -1;
                return CommandResult.Failed;
            }

            wield.TryWield(item, out Entity unwielded);

            if (Entity.PlayerControlled)
            {
                if (unwielded != null)
                    Locator.Log.Send(
                        $"You unwield {unwielded.ToSubjectString(false)}.",
                        Color.white);

                Locator.Log.Send($"You wield {item.ToSubjectString(false)}.",
                    Color.white);
            }

            cost = TurnScheduler.TurnTime;
            return CommandResult.Succeeded;
        }
    }
}
