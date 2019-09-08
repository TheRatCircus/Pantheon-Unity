// Base class for all actions in Pantheon's system of commands

using UnityEngine;
using Pantheon.Actors;
using Pantheon.Actions;

namespace Pantheon.Actions
{
    public abstract class BaseAction
    {
        // Callback which can be run after completion of an action
        public delegate void OnConfirm();
        public OnConfirm onConfirm = null;

        public Actor Actor { get; set; } = null;

        public BaseAction(Actor actor) => Actor = actor;

        // Assign this action to an actor, causing it to be called by
        // scheduler. Use for async actions; else just construct
        public virtual void AssignAction() => Actor.NextAction = this;

        /// <summary>
        /// Carry out this action's effects; only let scheduler call this.
        /// </summary>
        /// <returns>The action cost of this action.</returns>
        public abstract int DoAction();

        /// <summary>
        /// DoAction with a callback, invoked at an arbitrary point.
        /// </summary>
        /// <param name="onConfirm">Delegate invoked when this action is done.</param>
        /// <returns>The action cost of this action.</returns>
        public abstract int DoAction(OnConfirm onConfirm);
    }
}

// ActionWrapper holds an action in a ScriptableObject for serialization
public abstract class ActionWrapper : ScriptableObject
{
    public abstract BaseAction GetAction(Actor actor);
}