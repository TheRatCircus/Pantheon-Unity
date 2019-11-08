// WaitAction.cs
// Jerome Martina

using Pantheon.Actors;

namespace Pantheon.Actions
{
    /// <summary>
    /// An actor does nothing for a standard turn time.
    /// </summary>
    public sealed class WaitAction : Command
    {
        public WaitAction(Actor actor) : base(actor) { }

        /// <summary>
        /// Just do nothing.
        /// </summary>
        /// <returns>Game.TurnTime</returns>
        public override int DoAction() => Core.Game.TurnTime;

        // DoAction with callback
        public override int DoAction(OnConfirm onConfirm)
            => throw new System.NotImplementedException();

        public override string ToString()
            => $"{Actor.ActorName} is waiting.";
    }
}
