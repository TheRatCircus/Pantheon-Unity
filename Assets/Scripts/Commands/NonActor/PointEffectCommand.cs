// PointEffectCommand.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon.Core;
using Pantheon.World;
using System;

namespace Pantheon.Commands.NonActor
{
    /// <summary>
    /// Fire another command in/at a specific cell.
    /// </summary>
    [Serializable]
    public sealed class PointEffectCommand : NonActorCommand,
        ICellTargetedCommand, IRangedCommand
    {
        public Cell Cell { get; set; }
        public int Range { get; set; } = 5;
        [JsonProperty] private NonActorCommand cmd;

        public PointEffectCommand(Entity entity, NonActorCommand cmd)
            : base(entity)
        {
            this.cmd = cmd;
        }

        public override CommandResult Execute()
        {
            if (Cell != null)
            {
                cmd.Entity = Entity;
                if (cmd is ICellTargetedCommand ctc)
                    ctc.Cell = Cell;
                if (cmd is IEntityTargetedCommand etc)
                    etc.Target = Cell.Actor;
                cmd.Execute();
                return CommandResult.Succeeded;
            }

            if (Entity.PlayerControlled)
            {
                switch (Locator.Player.RequestCell(out Cell cell, Range))
                {
                    case InputMode.Cancelling:
                        return CommandResult.Cancelled;
                    case InputMode.Point:
                        return CommandResult.InProgress;
                    case InputMode.Default:
                        {
                            cmd.Entity = Entity;
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
