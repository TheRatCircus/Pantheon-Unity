// TurnScheduler.cs
// Courtesy of Dan Korostelev
// with modifications by Jerome Martina

#define DEBUG_SCHEDULER
#undef DEBUG_SCHEDULER

using Pantheon.Components.Entity;
using Pantheon.World;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Tilemaps;

namespace Pantheon.Core
{
    public sealed class TurnScheduler : MonoBehaviour, ITurnScheduler
    {
        public const int TurnTime = 100;

        [SerializeField] private Tile enemyTargetingTile = default;

        private IPlayer player;
        [SerializeField] private Tilemap enemyTargetingTilemap = default;

        // Once 100 energy has been spent by the player,
        // a turn is considered to have passed
        private int timeConsumed;
        [ReadOnly] [SerializeField] private int time;
        [ReadOnly] [SerializeField] private int turnProgress;
        public int Time => time;

        public List<Actor> Queue { get; } = new List<Actor>();
        private readonly HashSet<Cell> dirtyCells = new HashSet<Cell>();
        private readonly Dictionary<Vector2Int, int> enemyTargeting =
            new Dictionary<Vector2Int, int>();

        [ReadOnly] [SerializeField] private int lockCount;
        private Actor currentActor = null;
        private bool currentActorRemoved = false;

        public event Action ClockTickEvent; // 100 time units have passed
        public event Action<int, int> TimeChangeEvent; // Track time units passed in game
        public event Action<Actor> ActorDebugEvent;

        private void Start() => player = Locator.Player;

        private void Update()
        {
            for (int i = 0; i < Queue.Count; i++)
                if (!Tick())
                    break;
        }
#if ENERGY_SYSTEM
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

                SendActorDebugEvent(actor);

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

            Actor dequeued = Queue[0];
            Queue.RemoveAt(0);
            Queue.Add(dequeued);

            return true;
        }
#else
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

            Profiler.BeginSample("Scheduler: Act");
            if (actor.Control == ActorControl.Player)
            {
                currentActor = actor;
                int actionCost = actor.Act();
                currentActor = null;

                // Handle asynchronous input by returning -1
                if (actionCost < 0)
                {
                    Profiler.EndSample();
                    return false;
                }

                timeConsumed = actionCost;
                time += timeConsumed;
                TimeChangeEvent?.Invoke(time, timeConsumed);
                turnProgress += Mathf.FloorToInt(actionCost / TurnTime);
                if (turnProgress >= TurnTime)
                {
                    int turnsPassed = turnProgress / TurnTime;
                    turnProgress %= TurnTime;

                    for (int i = 0; i < turnsPassed; i++)
                        ClockTickEvent?.Invoke();
                }
                FOV.RefreshFOV(player.Entity.Level, player.Entity.Cell, true);
            }
            else
            {
                if (actor.Command == null)
                    actor.Entity.GetComponent<AI>().DecideCommand();
  
                actor.Progress += timeConsumed;
                if (actor.Progress >= actor.Threshold)
                {
                    currentActor = actor;
                    actor.Act();
                    currentActor = null;
                    actor.Progress = 0;
                }
            }
            Profiler.EndSample();

            if (currentActorRemoved)
            {
                currentActorRemoved = false;
                return true;
            }

            SendActorDebugEvent(actor);

            Profiler.BeginSample("Scheduler: Draw Dirty");
            RedrawDirtyCells(actor.Entity.Level);
            Profiler.EndSample();

            Actor dequeued = Queue[0];
            Queue.RemoveAt(0);
            Queue.Add(dequeued);

            return true;
        }
#endif

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

        public void TargetCell(Vector2Int pos)
        {
            if (enemyTargeting.TryGetValue(pos, out int value))
                value++;
            else
            {
                enemyTargeting.Add(pos, 1);
                enemyTargetingTilemap.SetTile((Vector3Int)pos, enemyTargetingTile);
            }
        }
        public void UntargetCell(Vector2Int pos)
        {
            if (enemyTargeting.TryGetValue(pos, out int value))
            {
                if (--value == 0)
                {
                    enemyTargeting.Remove(pos);
                    enemyTargetingTilemap.SetTile((Vector3Int)pos, null);
                }
            }
            else
                throw new ArgumentException($"Cell is not targeted.");
        }

        public void MarkDirtyCell(Cell cell) => dirtyCells.Add(cell);

        public void RedrawDirtyCells(Level level)
        {
            level.Draw(dirtyCells);
            dirtyCells.Clear();
        }

        [System.Diagnostics.Conditional("DEBUG_SCHEDULER")]
        private void SendActorDebugEvent(Actor actor)
        {
            ActorDebugEvent?.Invoke(actor);
        }
    }
}
