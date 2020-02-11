// ActorControlCommand.cs
// Jerome Martina

using Pantheon.Components.Entity;
using Pantheon.Utils;
using System;
using UnityEngine;
using ActorComp = Pantheon.Components.Entity.Actor;

namespace Pantheon.Commands.NonActor
{
    public sealed class ActorControlCommand : NonActorCommand, IEntityTargetedCommand
    {
        public Entity Target { get; set; } // Null if nothing possessing target
        private ActorControl newControl;

        public ActorControlCommand(Entity target, Entity possessor, ActorControl newControl)
            : base(target)
        {
            this.Target = possessor;
            this.newControl = newControl;
        }

        public override CommandResult Execute()
        {
            ActorComp actor = Entity.GetComponent<ActorComp>();
            switch (actor.Control)
            {
                case ActorControl.AI:
                    {
                        if (newControl == ActorControl.None)
                        {
                            Locator.Log.Send(
                                $"{Strings.Subject(Entity, true)} goes limp...",
                                Color.cyan);
                        }
                        else if (newControl == ActorControl.Player)
                        {
                            Locator.Log.Send(
                                $"You possess {Strings.Subject(Entity, false)}!",
                                Color.cyan);
                        }
                        else
                            throw new ArgumentException(
                                $"Both current and new controls are the same.");

                        break;
                    }
                case ActorControl.Player:
                    {
                        if (newControl == ActorControl.None)
                        {
                            Locator.Log.Send(
                                $"You lose control of your physical body!",
                                Color.magenta);

                        }
                        else if (newControl == ActorControl.AI)
                        {
                            Locator.Log.Send(
                                $"You are possessed by {Strings.Subject(Target, false)}!",
                                Color.magenta);
                        }
                        else
                            throw new ArgumentException(
                                $"Both current and new controls are the same.");

                        break;
                    }
                case ActorControl.None:
                    {
                        if (newControl == ActorControl.Player)
                        {
                            Locator.Log.Send(
                                $"You possess {Strings.Subject(Entity, false)}",
                                Color.cyan);
                        }
                        else if (newControl == ActorControl.AI)
                        {
                            Locator.Log.Send(
                                $"{Strings.Subject(Entity, true)} takes on a new vigour!",
                                Color.cyan);
                        }
                        else
                            throw new ArgumentException(
                                $"Both current and new controls are the same.");

                        break;
                    }
            }
            actor.Control = newControl;
            return CommandResult.Succeeded;
        }
    }
}
