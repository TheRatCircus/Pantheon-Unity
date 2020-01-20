// AI.cs
// Jerome Martina

#define DEBUG_AI
#undef DEBUG_AI

using Newtonsoft.Json;
using Pantheon.Commands.Actor;
using Pantheon.Utils;
using System;
using UnityEngine;
using UnityEngine.Profiling;

namespace Pantheon.Components
{
    [Serializable]
    public sealed class AI : EntityComponent
    {
        [JsonIgnore] public Entity Target { get; private set; }
        [JsonIgnore] public Entity[] Thralls { get; private set; }

        public AIUtility[] Utilities { get; private set; }

        [JsonIgnore] public Actor Actor { get; private set; }

        public AI(params AIUtility[] utilities) => Utilities = utilities;

        public void SetActor(Actor actor)
        {
            Actor = actor;
            actor.AIDecisionEvent += DecideCommand;
        }

        public void DecideCommand()
        {
            Profiler.BeginSample("AI");
            int max = 0;
            int i = -1;

            for (int j = 0; j < Utilities.Length; j++)
            {
                int util = Utilities[j].Recalculate(Entity, this);
                if (util > max)
                {
                    max = util;
                    i = j;
                }
            }
            
            if (i == -1)
            {
                Actor.Command = new WaitCommand(Entity);
                Profiler.EndSample();
                return;
            }

            AIUtility highest = Utilities[i];
            Actor.Command = highest.Invoke(Entity, this);
            DebugLogAI(highest, max);
            Profiler.EndSample();
        }

        public override void Receive(IComponentMessage msg)
        {
            switch (msg)
            {
                case DamageEventMessage dem:
                    Target = dem.Damager;
                    if (dem.Damager == Locator.Player.Entity)
                        Locator.Log.Send(
                            $"{Entity.ToSubjectString(true)} notices you!",
                            Colours._orange);
                    break;
                default:
                    break;
            }
        }

        public override EntityComponent Clone(bool full)
        {
            return new AI(Utilities);
        }

        [System.Diagnostics.Conditional("DEBUG_AI")]
        private void DebugLogAI(AIUtility highest, int util)
        {
            UnityEngine.Debug.Log(
                $"{Entity} AI - " +
                $"Highest utility: {highest} ({util}) - " +
                $"Cmd: {Actor.Command}");
        }
    }

    [Serializable]
    public abstract class AIUtility
    {
        public abstract int Recalculate(Entity entity, AI ai);
        public abstract ActorCommand Invoke(Entity entity, AI ai);
    }

    [Serializable]
    public sealed class ApproachUtility : AIUtility
    {
        public override int Recalculate(Entity entity, AI ai)
        {
            if (ai.Target == null)
                return 0;

            int dst = World.Level.Distance(entity.Position, ai.Target.Position);

            if (dst > 1)
                return 80;
            else
                return 0;
        }

        public override ActorCommand Invoke(Entity entity, AI ai)
        {
            Line path = entity.Level.GetPathTo(entity.Position, ai.Target.Position);
            if (path.Count < 0)
                return new WaitCommand(entity);
            else
                return new MoveCommand(entity, path[0]);
        }
    }

    [Serializable]
    public sealed class AttackUtility : AIUtility
    {
        public int Aggression { get; private set; } = 50;

        public AttackUtility(int aggression) => Aggression = aggression;

        public override int Recalculate(Entity entity, AI ai)
        {
            if (ai.Target == null)
                return 0;

            return Aggression;
        }

        public override ActorCommand Invoke(Entity entity, AI ai)
        {
            return new TalentCommand(entity, entity.GetComponent<Talents>().All[0]);
        }
    }

    [Serializable]
    public sealed class FleeUtility : AIUtility
    {
        public override int Recalculate(Entity entity, AI ai)
        {
            int health = entity.GetComponent<Health>().Current;
            double y = 1 / (1 + Math.Pow(Math.E, 4 * (health - .5f)));
            int utility = (int)Math.Floor(y);
            return utility;
        }

        public override ActorCommand Invoke(Entity entity, AI ai)
        {
            Vector2Int dir = entity.Level.FleeMap.RollDownhill(entity.Position);
            return new MoveCommand(entity, dir);
        }
    }
}
