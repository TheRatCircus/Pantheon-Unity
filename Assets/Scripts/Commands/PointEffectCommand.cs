// PointEffectCommand.cs
// Jerome Martina

using Pantheon.Core;
using Pantheon.World;
using System;

namespace Pantheon.Commands
{
    /// <summary>
    /// Fire another command in/at a specific cell.
    /// </summary>
    public sealed class PointEffectCommand : NonActorCommand
    {
        private int range = 5;
        private NonActorCommand cmd;

        public PointEffectCommand(Entity entity, NonActorCommand cmd)
            : base(entity)
        {
            this.cmd = cmd;
        }

        public override CommandResult Execute()
        {
            if (Entity.PlayerControlled)
            {
                switch (InputLocator._Svc.RequestCell(out Cell cell))
                {
                    case InputMode.Cancelling:
                        return CommandResult.Cancelled;
                    case InputMode.Point:
                        return CommandResult.InProgress;
                    case InputMode.Default:
                        {
                            // Point has come through
                            if (cmd is ICellTargetedCommand ctc)
                                ctc.Cell = cell;
                            if (cmd is IEntityTargetedCommand etc)
                                etc.Target = cell.Actor;
                            cmd.Execute();
                            return CommandResult.Succeeded;
                        }
                    default:
                        throw new Exception("Illegal InputMode returned.");
                }
            }
            else
                throw new NotImplementedException();
        }
    }
}
