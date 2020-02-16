// ApproachUtility.cs
// Jerome Martina

using Pantheon.Commands.Actor;
using Pantheon.Utils;

namespace Pantheon.Components.Entity
{
    using Entity = Pantheon.Entity;

    public sealed class ApproachUtility : AIUtility
    {
        public override int Recalculate(Entity entity, AI ai)
        {
            if (!ai.Alerted)
                return 0;

            int dst = Helpers.Distance(entity.Cell, Locator.Player.Entity.Cell);

            if (dst > 1)
                return 80;
            else
                return 0;
        }

        public override ActorCommand Invoke(Entity entity, AI ai)
        {
            Line path = entity.Level.GetPath(entity.Cell, Locator.Player.Entity.Cell);
            if (path.Count < 0)
                return new WaitCommand(entity);
            else
                return new MoveCommand(entity, path[0]);
        }
    }
}
