// IEntityTargetedCommand.cs
// Jerome Martina

namespace Pantheon.Commands
{
    public interface IEntityTargetedCommand
    {
        Entity Target { get; set; }
    }
}
