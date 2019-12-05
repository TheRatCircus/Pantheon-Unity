// ActorCommand.cs
// Jerome Martina

namespace Pantheon.Commands
{
    public abstract class ActorCommand : Command
    {
        public Actor Actor { get; private set; }
        public int Cost { get; private set; } // Energy penalty incurred on actor

        public ActorCommand(Actor actor, int cost)
        {
            Actor = actor;
            Cost = cost;
        }
    }
}

