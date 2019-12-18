// IPlayerControl.cs
// Jerome Martina

using Pantheon.World;
using System.Collections.Generic;

namespace Pantheon.Core
{
    public interface IPlayerControl
    {
        Entity PlayerEntity { get; }
        InputMode Mode { get; set; }
        HashSet<Entity> VisibleActors { get; }
        InputMode RequestCell(out Cell cell);
    }
}
