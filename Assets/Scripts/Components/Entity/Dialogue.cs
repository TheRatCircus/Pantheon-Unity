// Dialogue.cs
// Jerome Martina

using Pantheon.Utils;
using System;
using UnityEngine;

namespace Pantheon.Components.Entity
{
    [Serializable]
    public sealed class Dialogue : EntityComponent, IEntityDependentComponent
    {
        public string ID { get; set; }

        private void SpeakPerTurn()
        {
            if (!Entity.Visible)
                return;

            if (!RandomUtils.OneChanceIn(20))
                return;

            string line = DialogueHelpers.GetIdleDialogue(ID);
            line = line.Replace("@npc", Strings.Subject(Entity, false));
            line = line.Replace("@possessive", Strings.Possessive(Entity));
            line = line.FirstCharToUpper();
            Locator.Log.Send(line, Color.white);
        }

        public override EntityComponent Clone(bool full)
        {
            return new Dialogue() { ID = ID };
        }

        public void Initialize()
        {
            Locator.Scheduler.ClockTickEvent += SpeakPerTurn;
        }
    }
}
