// IPlayerControl.cs
// Jerome Martina

using Pantheon.World;

namespace Pantheon.Core
{
    public interface IPlayerControl
    {
        InputMode Mode { get; set; }
        System.Action<Cell> PointConfirmDelegate { get; set; }
    }
}
