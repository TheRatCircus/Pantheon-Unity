// InteractCommand.cs
// Jerome Martina

using Pantheon.Core;
using Pantheon.World;
using UnityEngine;

namespace Pantheon.Commands.Actor
{
    using Actor = Components.Entity.Actor;

    public sealed class InteractCommand : ActorCommand
    {
        public InteractCommand(Entity entity) : base(entity)
        {
            Cost = TurnScheduler.TurnTime;
        }

        public override CommandResult Execute()
        {
            if (Level.Connections.TryGetValue(Entity.Cell, out Connection conn))
            {
                GameController.Travel(conn);
                return CommandResult.Succeeded;
            }

            if (Actor.PlayerControlled(Entity))
                Locator.Log.Send(
                    "There's nothing to interact with here.",
                    Color.yellow);
            Cost = -1;
            return CommandResult.Failed;
        }
    }
}
