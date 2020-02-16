// AI.cs
// Jerome Martina

#define DEBUG_AI
#undef DEBUG_AI

using Pantheon.Commands.Actor;
using Pantheon.Content;
using Pantheon.Utils;
using System;
using System.Diagnostics;

namespace Pantheon.Components.Entity
{
    using Entity = Pantheon.Entity;

    [Serializable]
    public sealed class AI : EntityComponent, IEntityDependentComponent
    {
        public AIDefinition Definition { get; set; }
        public bool Alerted { get; private set; }
        private byte memory;

        public AI(AIDefinition def) => Definition = def;

        public void Initialize()
        {
            Entity.BecameVisibleEvent += OnBecomeVisible;
        }

        public void DecideCommand()
        {
            Actor actor = Entity.GetComponent<Actor>();

            if (Alerted && !Entity.Visible && --memory == 0)
            {
                // Tick down memory timer; if target is forgotten, unschedule
                // self and go back to sleep
                Alerted = false;
                Locator.Scheduler.RemoveActor(actor);
                actor.Command = null;
                DebugLogAI($"{Entity} has forgotten the player.");
                return;
            }

            int max = 0;
            int i = -1;

            for (int j = 0; j < Definition.Utilities.Length; j++)
            {
                int util = Definition.Utilities[j].Recalculate(Entity, this);
                if (util > max)
                {
                    max = util;
                    i = j;
                }
            }

            if (i == -1)
            {
                actor.Command = new WaitCommand(Entity);
                return;
            }

            AIUtility highest = Definition.Utilities[i];
            actor.Command = highest.Invoke(Entity, this);
            DebugLogAI(highest, max);
        }

        private void OnBecomeVisible()
        {
            Actor actor = Entity.GetComponent<Actor>();

            if (!Alerted)
            {
                if (!Definition.Peaceful)
                {
                    Locator.Log.Send(
                        $"{Strings.Subject(Entity, true)} notices you!",
                        Colours._orange);
                    Locator.Scheduler.AddActor(actor);
                    Alerted = true;
                    memory = Definition.Memory;
                }
                else
                {
                    actor.Command = new WaitCommand(Entity);
                    return;
                }
            }
        }

        public override EntityComponent Clone(bool full) => new AI(Definition);

        [Conditional("DEBUG_AI")]
        private void DebugLogAI(AIUtility highest, int util)
        {
            UnityEngine.Debug.Log(
                $"{Entity} AI - " +
                $"Highest utility: {highest} ({util}) - " +
                $"Cmd: {Entity.GetComponent<Actor>().Command}");
        }

        [Conditional("DEBUG_AI")]
        private void DebugLogAI(string msg)
        {
            UnityEngine.Debug.Log(msg);
        }
    }
}
