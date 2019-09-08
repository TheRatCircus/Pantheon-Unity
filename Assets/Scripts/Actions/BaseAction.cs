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
        public OnConfirm onConfirm;

        [System.NonSerialized]
        public Actor Actor; // Can be null

        // Constructor
        public BaseAction(Actor actor) => this.Actor = actor;

        // Assign this action to an actor, causing it to be called by
        // scheduler. Use for async actions; else just construct
        public virtual void AssignAction() => Actor.nextAction = this;

        // Carry out the effect of this action
        public abstract int DoAction();

        // DoAction with a callback
        public abstract int DoAction(OnConfirm onConfirm);
    }
}

// ActionWrapper holds an action in a ScriptableObject for serialization
public abstract class ActionWrapper : ScriptableObject
{
    public abstract BaseAction GetAction(Actor actor);
}