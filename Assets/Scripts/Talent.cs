// Talent.cs
// Jerome Martina

using Pantheon.Commands;
using Pantheon.Commands.NonActor;
using Pantheon.World;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon
{
    [System.Serializable]
    public sealed class Talent
    {
        public int Range { get; set; }
        public NonActorCommand[] Commands { get; set; }

        public Talent(int range, params NonActorCommand[] commands)
        {
            Range = range;
            Commands = commands;
            foreach (NonActorCommand cmd in Commands)
            {
                if (cmd is IRangedCommand rc)
                    rc.Range = Range;
            }
        }

        public CommandResult Cast(Entity evoker)
        {
            CommandResult result = CommandResult.Succeeded;

            foreach (NonActorCommand nac in Commands)
            {
                nac.Entity = evoker;
                CommandResult r = nac.Execute();

                if (r == CommandResult.InProgress)
                    result = CommandResult.InProgress;
                if (r == CommandResult.Cancelled)
                    result = CommandResult.Cancelled;
            }

            return result;
        }

        public CommandResult Cast(Entity evoker,
            Cell cell, List<Cell> line, List<Vector2Int> path)
        {
            CommandResult result = CommandResult.Succeeded;

            foreach (NonActorCommand nac in Commands)
            {
                nac.Entity = evoker;

                if (nac is IEntityTargetedCommand etc)
                    etc.Target = cell.Actor;
                if (nac is ICellTargetedCommand ctc)
                    ctc.Cell = cell;
                if (nac is ILineTargetedCommand ltc)
                    ltc.Line = line;

                CommandResult r = nac.Execute();

                if (r == CommandResult.InProgress)
                    result = CommandResult.InProgress;
                if (r == CommandResult.Cancelled)
                    result = CommandResult.Cancelled;
            }

            return result;
        }
    }
}