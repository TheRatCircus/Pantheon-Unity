// ICellTargetedCommand.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon.Commands
{
    public interface ICellTargetedCommand
    {
        Vector2Int Cell { get; set; }
    }
}
