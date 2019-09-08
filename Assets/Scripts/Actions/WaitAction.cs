// Do nothing for the time of a standard turn

using Pantheon.Actors;

namespace Pantheon.Actions
{
    public class WaitAction : BaseAction
    {
        // Constructor
        public WaitAction(Actor actor) : base(actor) { }

        // Wait for the time of a standard turn
        public override int DoAction() => Core.Game.TurnTime;

        // DoAction with callback
        public override int DoAction(OnConfirm onConfirm)
            => throw new System.NotImplementedException();
    }
}
