// PointEffectCommand.cs
// Jerome Martina

using Pantheon.Core;
using Pantheon.World;

namespace Pantheon.Commands
{
    /// <summary>
    /// Fire another command in/at a specific cell.
    /// </summary>
    public sealed class PointEffectCommand : NonActorCommand
    {
        private int range = 5;
        private NonActorCommand cmd;

        public PointEffectCommand(Entity entity, NonActorCommand cmd)
            : base(entity)
        {
            this.cmd = cmd;
        }

        public override void Execute()
        {
            if (Entity.PlayerControlled)
            {
                InputLocator._Svc.Mode = InputMode.Point;
                InputLocator._Svc.PointConfirmDelegate = delegate (Cell c)
                {
                    if (cmd is ICellTargetedCommand ctc)
                        ctc.Cell = c;
                    if (cmd is IEntityTargetedCommand etc)
                        etc.Target = c.Actor;

                    cmd.Execute();
                };
            }
            else
                throw new System.NotImplementedException();
        }
    }
}
