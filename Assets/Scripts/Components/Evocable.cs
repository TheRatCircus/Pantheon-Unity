// Evocable.cs
// Jerome Martina

using Pantheon.Commands;
using Pantheon.Core;
using System;

namespace Pantheon.Components
{
    [Serializable]
    public sealed class Evocable : EntityComponent
    {
        public int EvokeTime { get; set; } = TurnScheduler.TurnTime;
        private Talent[] talents;
        public Talent[] Talents { get => talents; set => talents = value; }

        public Evocable(params Talent[] talents) => this.talents = talents;

        public CommandResult Evoke(Entity evoker, int talent)
        {
            return talents[talent].Cast(evoker);
        }

        public void AddTalent(Talent talent)
        {
            Array.Resize(ref talents, talents.Length + 1);
            talents[talents.GetUpperBound(0)] = talent;
        }

        public override EntityComponent Clone(bool full)
        {
            throw new NotImplementedException();
        }
    }
}
