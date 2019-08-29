// 
namespace Pantheon.Actions
{
    public abstract class BaseAction
    {
        public Actor actor;
        public int ActionCost;

        public BaseAction(Actor actor)
        {
            this.actor = actor;
        }

        public virtual void AssignAction() => actor.nextAction = this;
        public abstract int DoAction();
    }
}
