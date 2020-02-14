// PointEffectCommand.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon.Core;
using Pantheon.World;
using System;
using UnityEngine;

namespace Pantheon.Commands.NonActor
{
    using Actor = Components.Entity.Actor;

    /// <summary>
    /// Fire another command in/at a specific cell.
    /// </summary>
    [Serializable]
    public sealed class PointEffectCommand : NonActorCommand,
        ICellTargetedCommand, IRangedCommand
    {
        public Vector2Int Cell { get; set; }
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
                    etc.Target = Level.ActorAt(Cell);
                cmd.Execute();
                return CommandResult.Succeeded;
            }

            if (Actor.PlayerControlled(Entity))
            {
                switch (Locator.Player.RequestCell(out Vector2Int cell, Range))
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
                                etc.Target = Level.ActorAt(cell);
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
