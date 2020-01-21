// ICellTargetedCommand.cs
// Jerome Martina

using Pantheon.World;

namespace Pantheon.Commands
{
    public interface ICellTargetedCommand
    {
        Cell Cell { get; set; }
    }
}
