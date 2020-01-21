// TurnScheduler.cs
// Courtesy of Dan Korostelev
// with modifications by Jerome Martina

#define DEBUG_SCHEDULER
#undef DEBUG_SCHEDULER

using Pantheon.Components;
using Pantheon.World;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace Pantheon.Core
{
    public sealed class TurnScheduler : MonoBehaviour, ITurnScheduler
    {
        public const int TurnTime = 100;

        private IPlayer player;

        // Once 100 energy has been spent by the player,
        // a turn is considered to have passed
        [ReadOnly] [SerializeField] private int turnProgress = 0;
        [ReadOnly] [SerializeField] private int turns = 0;
        public int Turns => turns;

        private List<Actor> queue = new List<Actor>();
        public List<Actor> Queue => queue;
        private HashSet<Cell> dirtyCells = new HashSet<Cell>();

        [ReadOnly] [SerializeField] private int lockCount = 0;
        private Actor currentActor = null;
        private bool currentActorRemoved = false;

        public event Action ClockTickEvent; // 100 time units have passed
        public event Action<int> TimeChangeEvent; // Track time units passed in game
        public event Action<int> PlayerActionEvent;
        public event Action<Actor> ActorDebugEvent;
        public event Action ActionDoneEvent;

        private void Start() => player = Locator.Player;

        private void Update()
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
                Profiler.BeginSample("Scheduler: Act()");
                currentActor = actor;
                int actionCost = actor.Act();
                currentActor = null;
                Profiler.EndSample();

                if (currentActorRemoved)
                {
                    currentActorRemoved = false;
                    return true;
                }

                if (actionCost == 0)
                    UnityEngine.Debug.LogWarning(
                        "A command with 0 energy cost was scheduled.");

#if DEBUG_SCHEDULER
                ActorDebugEvent?.Invoke(actor);
#endif

                // Handle asynchronous input by returning -1
                if (actionCost < 0)
                    return false;

                // An action has just been done
                actor.Energy -= actionCost;
                ActionDoneEvent?.Invoke();

                Profiler.BeginSample("Scheduler: Draw Dirty");
                actor.Entity.Level.Draw(dirtyCells);
                dirtyCells.Clear();
                Profiler.EndSample();

                Profiler.BeginSample("Scheduler: Player");
                if (actor.Control == ActorControl.Player)
                {
                    FOV.RefreshFOV(player.Entity.Level, player.Entity.Cell, true);
                    
                    float speedFactor = actor.Speed / TurnTime;
                    turnProgress += Mathf.FloorToInt(actionCost / speedFactor);
                    if (turnProgress >= TurnTime)
                    {
                        int turnsPassed = turnProgress / TurnTime;
                        turns += turnsPassed;
                        turnProgress %= TurnTime;

                        for (int i = 0; i < turnsPassed; i++)
                            ClockTickEvent?.Invoke();

                        TimeChangeEvent?.Invoke(turns);
                    }
                    // Signals a successful player action to HUD
                    PlayerActionEvent?.Invoke(actor.Energy);
                }
                Profiler.EndSample();
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

        public void MarkDirtyCell(Cell cell) => dirtyCells.Add(cell);
    }
}
