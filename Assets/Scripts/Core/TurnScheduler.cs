// TurnScheduler.cs
// Courtesy of Dan Korostelev

using Pantheon.ECS.Components;
using Pantheon.World;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.Core
{
    public sealed class TurnScheduler : MonoBehaviour
    {
        public const int TurnTime = 100;

        private GameController ctrl;

        // Once 100 energy has been spent by the player,
        // a turn is considered to have passed
        [ReadOnly] [SerializeField] private int turnProgress = 0;
        [ReadOnly] [SerializeField] private int turns = 0;
        public List<Actor> Queue { get; } = new List<Actor>();
        [ReadOnly] [SerializeField] private int lockCount = 0;
        private Actor currentActor = null;
        private bool currentActorRemoved = false;

        public event Action<int> AITurnEvent; // Send to AISystem
        public event Action TurnChangeEvent;
        public event Action<int> ClockTickEvent;
        public event Action<int> PlayerActionEvent;
        public event Action<Actor> ActorDebugEvent;
        public event Action ActionDoneEvent;

        private void Start() => ctrl = GetComponent<GameController>();

        void Update()
        {
            for (int i = 0; i < Queue.Count; i++)
                if (!Tick())
                    break;
        }

        // Iterate through each actor in the queue, and
        // take its actions until its energy is spent
        private bool Tick()
        {
            if (lockCount > 0)
                return false;

            Actor actor = Queue[0];

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

                switch (actor.Control)
                {
                    case ActorControl.Player:
                        // Either PlayerControl sends an action or does not
                        break;
                    case ActorControl.AI:
                        AITurnEvent.Invoke(actor.GUID);
                        break;
                    case ActorControl.None:
                        Actor deq = Queue[0];
                        Queue.RemoveAt(0);
                        Queue.Add(deq);
                        return true;
                }

                int actionCost = actor.Act();
                currentActor = null;

                if (currentActorRemoved)
                {
                    currentActorRemoved = false;
                    return true;
                }

                if (actionCost == 0)
                    UnityEngine.Debug.LogWarning(
                        "A command with 0 energy cost was scheduled.");

                ActorDebugEvent?.Invoke(actor);

                // Handle asynchronous input by returning -1
                if (actionCost < 0)
                    return false;

                // An action has just been done
                actor.Energy -= actionCost;
                ActionDoneEvent?.Invoke();
                
                // If actor was player or visible, refresh FOV
                if (actor.Control == ActorControl.Player ||
                    ctrl.EntityManager.CellOf(actor).Visible)
                {
                    HashSet<Cell> refreshed = FOV.RefreshFOV(
                        ctrl.World.ActiveLevel,
                        ctrl.EntityManager.CellOf(ctrl.EntityManager.Player).Position, true);
                    //ctrl.PlayerControl.RecalculateVisible(refreshed);
                }

                if (actor.Control == ActorControl.Player)
                {
                    turnProgress += actionCost;
                    if (turnProgress >= TurnTime)
                    {
                        int turnsPassed = turnProgress / TurnTime;
                        turns += turnsPassed;
                        turnProgress %= TurnTime;

                        for (int i = 0; i < turnsPassed; i++)
                            TurnChangeEvent?.Invoke();
                        ClockTickEvent?.Invoke(turns);
                    }
                    // Signals a successful player action to HUD
                    PlayerActionEvent?.Invoke(actor.Energy);
                }
                // Action may have added a lock
                if (lockCount > 0)
                    return false;
            }

            // Give the actor their speed value's worth of energy back
            actor.Energy += actor.Speed;

            // Update HUD again to reflect refill
            if (actor.Control == ActorControl.Player)
                PlayerActionEvent?.Invoke(actor.Energy);

            Actor dequeued = Queue[0];
            Queue.RemoveAt(0);
            Queue.Add(dequeued);

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
        public void AddActor(Actor actor) => Queue.Add(actor);
        public void RemoveActor(Actor actor)
        {
            Queue.Remove(actor);

            if (currentActor == actor)
                currentActorRemoved = true;
        }
    }
}
