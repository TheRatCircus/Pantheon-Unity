// ILineTargetedCommand.cs
// Jerome Martina

using Pantheon.Utils;

namespace Pantheon.Commands
{
    public interface ILineTargetedCommand
    {
        Line Line { get; set; }
    }
}