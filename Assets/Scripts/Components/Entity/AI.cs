// AI.cs
// Jerome Martina

#define DEBUG_AI
#undef DEBUG_AI

using Newtonsoft.Json;
using Pantheon.Commands.Actor;
using Pantheon.Utils;
using Pantheon.World;
using System;
using UnityEngine;

namespace Pantheon.Components.Entity
{
    using Entity = Pantheon.Entity;

    [Serializable]
    public sealed class AI : EntityComponent
    {
        private Entity target;
        [JsonIgnore] public Entity Target
        {
            get => target;
            private set
            {
                if (target != null)
                    target.DestroyedEvent -= ClearTarget;
                target = value;
                if (value != null)
                    target.DestroyedEvent += ClearTarget;
            }
        }
        [JsonIgnore] public Entity[] Thralls { get; private set; }
        [JsonIgnore] public Vector2Int Destination { get; set; }

        public bool Peaceful { get; set; }
        public AIStrategy Strategy { get; set; }
        public AIUtility[] Utilities { get; set; }

        [JsonIgnore] public Actor Actor { get; set; }

        public AI(params AIUtility[] utilities) => Utilities = utilities;

        public void DecideCommand()
        {
            if (!Peaceful && Entity.Visible && Target == null)
            {
                Target = Locator.Player.Entity;
                Locator.Log.Send(
                    $"{Strings.Subject(Entity, true)} notices you!",
                    Colours._orange);
            }

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
                return;
            }

            AIUtility highest = Utilities[i];
            Actor.Command = highest.Invoke(Entity, this);
            DebugLogAI(highest, max);
        }

        public override void Receive(IComponentMessage msg)
        {
            switch (msg)
            {
                case DamageEventMessage dem:
                    RespondToDamage(dem.Damager);
                    break;
                default:
                    break;
            }
        }

        private void RespondToDamage(Entity damager)
        {
            if (Target == damager)
                return;

            Target = damager;
            if (damager == Locator.Player.Entity)
                Locator.Log.Send(
                    $"{Strings.Subject(Entity, true)} notices you!",
                    Colours._orange);
        }

        private void ClearTarget()
        {
            Target.DestroyedEvent -= ClearTarget;
            Target = null;
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
}
