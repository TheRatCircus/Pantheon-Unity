// ActorSystem.cs
// Jerome Martina
// Turn scheduler courtesy of Dan Korostelev

using Pantheon.Commands;
using Pantheon.Core;
using Pantheon.ECS.Components;
using Pantheon.UI;
using Pantheon.Utils;
using Pantheon.World;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Pantheon.ECS.Systems
{
    public sealed class ActorSystem : ComponentSystem
    {
        public const int TurnTime = 100; // One standard turn

        private GameLog log;

        // Once 100 energy has been spent by the player,
        // a turn is considered to have passed
        [ReadOnly] [SerializeField] private int turnProgress = 0;
        [ReadOnly] [SerializeField] private int turns = 0;

        private List<Actor> queue = null;
        [ReadOnly] [SerializeField] private int lockCount = 0;
        private Actor currentActor = null;
        private bool currentActorRemoved = false;

        public event Action TurnChangeEvent;
        public event Action<int> ClockTickEvent;
        public event Action<int> PlayerActionEvent;
        public event Action<Actor> ActorDebugEvent;
        public event Action ActionDoneEvent;

        public ActorSystem(EntityManager mgr, GameLog log)
            : base(mgr)
        {
            queue = mgr.ActorComponents;
            this.log = log;
        }

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
                        Entity e = mgr.GetEntity(actor.GUID);
                        Player p = e.GetComponent<Player>();
                        if (p.AutoMovePath.Count > 0)
                        {
                            actor.Command = new MoveCommand(e, p.AutoMovePath[0], TurnTime);
                            p.AutoMovePath.RemoveAt(0);
                        }
                        break;
                    case ActorControl.AI:
                        UpdateAI(actor);
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
                        "A command with 0 energy cost was scheduled");

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
                            TurnChangeEvent?.Invoke();
                        ClockTickEvent?.Invoke(turns);
                    }
                    // Signals a successful player action to HUD
                    PlayerActionEvent?.Invoke(actor.Energy);
                    Position pos = mgr.Player.GetComponent<Position>();
                    FOV.RefreshFOV(pos.Level, pos.Cell.Position);
                }
                // Action may have added a lock
                if (lockCount > 0)
                    return false;
            }

            // Give the actor their speed value's worth of energy back
            actor.Energy += actor.Speed;

            // Update HUD again to reflect refill
            if (actor.PlayerControlled)
                PlayerActionEvent?.Invoke(actor.Energy);

            Actor dequeued = queue[0];
            queue.RemoveAt(0);
            queue.Add(dequeued);

            return true;
        }

        // AI is updated sequentially, and thus has no system
        public void UpdateAI(Actor actor)
        {
            Entity e = mgr.GetEntity(actor.GUID);
            AI ai = e.GetComponent<AI>();
            Position pos = e.GetComponent<Position>();

            // Random energy
            int r = Random.Range(0, 21);
            if (r >= 18)
                actor.Energy += actor.Speed / 10;
            else if (r <= 2)
                actor.Energy -= actor.Speed / 10;

            if (ai.Target != null) // Player detected
            {
                Cell targetCell = ai.Target.GetComponent<Position>().Cell;

                if (pos.Level.AdjacentTo(pos.Cell, targetCell))
                    actor.Command = new MeleeCommand(e, targetCell);
                else
                    actor.Command = MoveCommand.MoveOrWait(e, targetCell);
            }
            else // Player not encountered yet
            {
                if (pos.Cell.Visible) // Detect player and begin approach
                {
                    ai.Target = mgr.Player;
                    actor.Command = MoveCommand.MoveOrWait(e, 
                        ai.Target.GetComponent<Position>().Cell);
                    log.Send($"The {e.Name} notices you!", Colours._orange);
                }
                else // Sleep
                    actor.Command = new WaitCommand(e);
            }
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