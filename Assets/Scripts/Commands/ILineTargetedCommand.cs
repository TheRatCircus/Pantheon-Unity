// ILineTargetedCommand.cs
// Jerome Martina

using Pantheon.World;
using System.Collections.Generic;

namespace Pantheon.Commands
{
    public interface ILineTargetedCommand
    {
        List<Cell> Line { get; set; }
    }
}