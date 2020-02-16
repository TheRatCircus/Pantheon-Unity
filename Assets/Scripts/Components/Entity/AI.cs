// AI.cs
// Jerome Martina

#define DEBUG_AI
#undef DEBUG_AI

using Pantheon.Commands.Actor;
using Pantheon.Content;
using Pantheon.Utils;
using System;

namespace Pantheon.Components.Entity
{
    using Entity = Pantheon.Entity;

    [Serializable]
    public sealed class AI : EntityComponent
    {
        public AIDefinition Definition { get; set; }
        public bool Alerted { get; private set; }

        public AI(AIDefinition def) => Definition = def;

        public void DecideCommand()
        {
            Actor actor = Entity.GetComponent<Actor>();

            if (!Definition.Peaceful && Entity.Visible && !Alerted)
            {
                Locator.Log.Send(
                    $"{Strings.Subject(Entity, true)} notices you!",
                    Colours._orange);
                Alerted = true;
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

        public override EntityComponent Clone(bool full) => new AI(Definition);

        [System.Diagnostics.Conditional("DEBUG_AI")]
        private void DebugLogAI(AIUtility highest, int util)
        {
            UnityEngine.Debug.Log(
                $"{Entity} AI - " +
                $"Highest utility: {highest} ({util}) - " +
                $"Cmd: {Entity.GetComponent<Actor>().Command}");
        }
    }
}
