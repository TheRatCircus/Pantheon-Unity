// CellFlag.cs
// Jerome Martina

using System;

namespace Pantheon.World
{
    [Flags]
    public enum CellFlag
    {
        Visible = (1 << 0),
        Revealed = (1 << 1)
    }
}
