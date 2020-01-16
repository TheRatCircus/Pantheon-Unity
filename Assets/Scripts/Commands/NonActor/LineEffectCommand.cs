// LineEffectCommand.cs
// Jerome Martina

using Pantheon.Core;
using Pantheon.World;
using System;
using System.Collections.Generic;

namespace Pantheon.Commands.NonActor
{
    /// <summary>
    /// Fire another command in a line of cells.
    /// </summary>
    [Serializable]
    public sealed class LineEffectCommand : NonActorCommand,
        ILineTargetedCommand, IRangedCommand
    {
        public List<Cell> Line { get; set; }
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
                    foreach (Cell c in Line)
                        etc.Target = c.Actor;
                    cmd.Execute();
                }
                if (cmd is ICellTargetedCommand ctc)
                {
                    foreach (Cell c in Line)
                        ctc.Cell = c;
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
                switch (Locator.Player.RequestLine(out List<Cell> line, Range))
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
                                foreach (Cell c in line)
                                    etc.Target = c.Actor;
                                cmd.Execute();
                            }
                            if (cmd is ICellTargetedCommand ctc)
                            {
                                foreach (Cell c in line)
                                    ctc.Cell = c;
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
