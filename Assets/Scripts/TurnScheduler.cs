// TurnScheduler.cs
// Jerome Martina

using Pantheon.Commands;
using Pantheon.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon
{
    public sealed class TurnScheduler : MonoBehaviour
    {
        public const int TurnTime = 100;

        private GameController ctrl;

        // Once 100 energy has been spent by the player,
        // a turn is considered to have passed
        [ReadOnly] [SerializeField] private int turnProgress = 0;
        [ReadOnly] [SerializeField] private int turns = 0;

        private List<Actor> queue = new List<Actor>();
        [ReadOnly] [SerializeField] private int lockCount = 0;
        private Actor currentActor = null;
        private bool currentActorRemoved = false;

        public event Action TurnChangeEvent;
        public event Action<int> ClockTickEvent;
        public event Action<int> PlayerActionEvent;
        public event Action<Actor> ActorDebugEvent;
        public event Action ActionDoneEvent;

        private void Start()
        {
            ctrl = GetComponent<GameController>();
        }

        void Update()
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
                switch (actor.Control)
                {
                    case ActorControl.None: // Skip any uncontrolled actor
                        Actor a = queue[0];
                        queue.RemoveAt(0);
                        queue.Add(a);
                        return true;
                    case ActorControl.Player:
                        Player player = actor.GetComponent<Player>();
                        if (player.AutoMovePath.Count > 0)
                        {
                            actor.Command = new MoveCommand(actor, player.AutoMovePath[0], TurnTime);
                            player.AutoMovePath.RemoveAt(0);
                        }
                        break;
                    case ActorControl.AI:
                        AI ai = actor.GetComponent<AI>();
                        ai.DecideCommand();
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
                    UnityEngine.Debug.LogWarning(
                        "A command with 0 energy cost was scheduled.");

                ActorDebugEvent?.Invoke(actor);

                // Handle asynchronous input by returning -1
                if (actionCost < 0)
                    return false;

                // An action has just been done
                actor.Energy -= actionCost;
                ActionDoneEvent?.Invoke();

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
                    FOV.RefreshFOV(actor.Level, actor.Cell.Position);
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
