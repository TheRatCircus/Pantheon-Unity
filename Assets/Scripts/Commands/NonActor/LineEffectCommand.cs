// LineEffectCommand.cs
// Jerome Martina

using Pantheon.Core;
using Pantheon.Utils;
using Pantheon.World;
using System;
using UnityEngine;

namespace Pantheon.Commands.NonActor
{
    /// <summary>
    /// Fire another command in a line of cells.
    /// </summary>
    [Serializable]
    public sealed class LineEffectCommand : NonActorCommand,
        ILineTargetedCommand, IRangedCommand
    {
        [NonSerialized] private Line line;
        public Line Line { get => line; set => line = value; }
        public int Range { get; set; } = 5;
        private NonActorCommand cmd;

        public LineEffectCommand(Entity entity, NonActorCommand cmd)
            : base(entity)
        {
            this.cmd = cmd;
        }

        public override CommandResult Execute()
        {
            if (Line != null)
            {
                cmd.Entity = Entity;
                if (cmd is IEntityTargetedCommand etc)
                {
                    Level level = Entity.Level;
                    foreach (Vector2Int cell in Line)
                        etc.Target = level.ActorAt(cell);
                    cmd.Execute();
                }
                if (cmd is ICellTargetedCommand ctc)
                {
                    foreach (Vector2Int cell in Line)
                        ctc.Cell = cell;
                    cmd.Execute();
                }
                if (cmd is ILineTargetedCommand ltc)
                {
                    ltc.Line = Line;
                    cmd.Execute();
                }

                return CommandResult.Succeeded;
            }

            if (Entity.PlayerControlled)
            {
                switch (Locator.Player.RequestLine(out Line line, Range))
                {
                    case InputMode.Cancelling:
                        return CommandResult.Cancelled;
                    case InputMode.Line:
                        return CommandResult.InProgress;
                    case InputMode.Default:
                        {
                            // Line has come through
                            cmd.Entity = Entity;
                            if (cmd is IEntityTargetedCommand etc)
                            {
                                Level level = Entity.Level;
                                foreach (Vector2Int cell in line)
                                    etc.Target = level.ActorAt(cell);
                                cmd.Execute();
                            }
                            if (cmd is ICellTargetedCommand ctc)
                            {
                                foreach (Vector2Int cell in line)
                                    ctc.Cell = cell;
                                cmd.Execute();
                            }
                            if (cmd is ILineTargetedCommand ltc)
                            {
                                ltc.Line = line;
                                cmd.Execute();
                            }
                            
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
