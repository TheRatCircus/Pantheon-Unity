// AISystem.cs
// Jerome Martina

using Pantheon.Core;
using Pantheon.ECS.Components;
using Pantheon.UI;
using Pantheon.Utils;
using Pantheon.World;
using UnityEngine;

namespace Pantheon.ECS.Systems
{
    public sealed class AISystem : ComponentSystem
    {
        public AISystem(EntityManager mgr, GameLog log, TurnScheduler scheduler)
            : base(mgr, log) => scheduler.AITurnEvent += ProcessAI;

        public override void Update()
            => throw new System.NotImplementedException();

        private void ProcessAI(int guid)
        {
            Actor actor = mgr.Actors[guid];

            // Random energy
            int r = Random.Range(0, 21);
            if (r >= 18)
                actor.Energy += actor.Speed / 10;
            else if (r <= 2)
                actor.Energy -= actor.Speed / 10;

            AI ai = mgr.AI[guid];
            if (ai == null)
                throw new System.Exception(
                    "AI not bound to AI-controlled entity.");

            Location loc = mgr.Locations[guid];
            Level level = loc.Level;

            if (ai.Target != null)
            {
                Cell targetCell = mgr.Locations[ai.Target.GUID].Cell;

                if (level.AdjacentTo(loc.Cell, targetCell))
                {
                    // Melee
                    actor.ActionCost = TurnScheduler.TurnTime;
                }
                else
                {
                    loc.Move(level, level.GetPathTo(loc.Cell, targetCell)[0]);
                    actor.ActionCost = TurnScheduler.TurnTime;
                }
            }
            else // Player not encountered yet
            {
                if (loc.Cell.Visible) // Detect player and begin approach
                {
                    ai.Target = mgr.Player;
                    loc.Move(level, level.GetPathTo(
                        loc.Cell,
                        mgr.Locations[ai.Target.GUID].Cell)[0]);
                    log.Send(
                        $"The {mgr.Entities[guid].Name} notices you!",
                        Colours._orange);
                    actor.ActionCost = TurnScheduler.TurnTime;
                }
                else
                    actor.ActionCost = TurnScheduler.TurnTime;
            }
        }
    }
}
