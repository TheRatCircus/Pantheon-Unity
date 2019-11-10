// ActorSystem.cs
// Jerome Martina
// Turn scheduler courtesy of Dan Korostelev

using Pantheon.ECS.Components;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.ECS.Systems
{
    public sealed class ActorSystem : ComponentSystem
    {
        public const int TurnTime = 100; // One standard turn

        // Once 100 energy has been spent by the player,
        // a turn is considered to have passed
        [ReadOnly] [SerializeField] private int turnProgress = 0;
        [ReadOnly] [SerializeField] private int turns = 0;
        [ReadOnly] [SerializeField] private int lockCount = 0;
        private List<Actor> queue = new List<Actor>();
        private Actor currentActor = null;
        private bool currentActorRemoved = false;

        // Events
        public event Action OnTurnChangeEvent;
        public event Action<int> OnClockTickEvent;
        public event Action<int> OnPlayerActionEvent;
        public event Action<Actor> ActorDebugEvent;
        public event Action ActionDoneEvent;

        public override void UpdateComponents()
        {
            for (int i = 0; i < queue.Count; i++)
                if (!Tick())
                    break;
        }

        // Iterate through each actor in the queue, and
        // take its actions until its energy is spent
        private bool Tick()
        {
            if (lockCount > 0)
                return false;

            if (queue.Count <= 0)
                throw new Exception(
                    "Turn queue should not be empty");

            Actor actor = queue[0];
            if (actor == null)
                return false;

            if (currentActorRemoved)
            {
                currentActorRemoved = false;
                return true;
            }

            while (actor.Energy > 0)
            {
                currentActor = actor;
                switch (actor.ActorControl)
                {
                    case ActorControl.None:
                        return false;
                    case ActorControl.Player:
                        break;
                    case ActorControl.AI:
                        actor.RequestAICommand();
                        break;
                }

                int actionCost = actor.Act();
                currentActor = null;

                if (currentActorRemoved)
                {
                    currentActorRemoved = false;
                    return true;
                }

                if (actionCost == 0)
                    UnityEngine.Debug.LogWarning
                        ("An action with 0 energy cost was scheduled");

                ActorDebugEvent?.Invoke(actor);

                // Handle asynchronous input by returning -1
                if (actionCost < 0)
                    return false;

                // An action has just been done
                actor.Energy -= actionCost;
                ActionDoneEvent?.Invoke();

                if (actor.PlayerControlled)
                {
                    turnProgress += actionCost;
                    if (turnProgress >= TurnTime)
                    {
                        int turnsPassed = turnProgress / TurnTime;
                        turns += turnsPassed;
                        turnProgress %= TurnTime;

                        for (int i = 0; i < turnsPassed; i++)
                            OnTurnChangeEvent?.Invoke();
                        OnClockTickEvent?.Invoke(turns);
                    }
                    // Signals a successful player action to HUD
                    OnPlayerActionEvent?.Invoke(actor.Energy);
                }
                // Action may have added a lock
                if (lockCount > 0)
                    return false;
            }

            // Give the actor their speed value's worth of energy back
            actor.Energy += actor.Speed;

            // Update HUD again to reflect refill
            if (actor.PlayerControlled)
                OnPlayerActionEvent?.Invoke(actor.Energy);

            Actor dequeued = queue[0];
            queue.RemoveAt(0);
            queue.Add(dequeued);

            return true;
        }

        // Temporarily lock and then unlock the scheduler recursively to allow
        // things to happen between ticks
        public void Lock() => lockCount++;
        public void Unlock()
        {
            if (lockCount == 0)
                throw new Exception("Turn scheduler is not locked.");

            lockCount--;
        }

        // Add and remove actors from the queue
        public void AddActor(Actor actor) => queue.Add(actor);
        public void RemoveActor(Actor actor)
        {
            queue.Remove(actor);

            if (currentActor == actor)
                currentActorRemoved = true;
        }
    }
}