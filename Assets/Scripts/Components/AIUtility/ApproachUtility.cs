// ApproachUtility.cs
// Jerome Martina

using Pantheon.Commands.Actor;
using Pantheon.Utils;
using System;

namespace Pantheon.Components.Entity
{
    using Entity = Pantheon.Entity;

    [Serializable]
    public sealed class ApproachUtility : AIUtility
    {
        public override int Recalculate(Entity entity, AI ai)
        {
            if (ai.Target == null)
                return 0;

            int dst = entity.Level.Distance(entity.Cell, ai.Target.Cell);

            if (dst > 1)
                return 80;
            else
                return 0;
        }

        public override ActorCommand Invoke(Entity entity, AI ai)
        {
            Line path = entity.Level.GetPath(entity.Cell, ai.Target.Cell);
            if (path.Count < 0)
                return new WaitCommand(entity);
            else
                return new MoveCommand(entity, path[0]);
        }
    }
}
