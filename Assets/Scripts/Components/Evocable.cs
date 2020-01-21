// Evocable.cs
// Jerome Martina

using Pantheon.Commands;
using Pantheon.Core;
using Pantheon.World;
using System;
using System.Collections.Generic;

namespace Pantheon.Components
{
    [Serializable]
    public sealed class Evocable : EntityComponent
    {
        public int EvokeTime { get; set; } = TurnScheduler.TurnTime;
        private Talent[] talents;
        public Talent[] Talents { get => talents; set => talents = value; }

        public Evocable(int evokeTime = TurnScheduler.TurnTime, 
            params Talent[] talents) => this.talents = talents;

        public CommandResult Evoke(Entity evoker, int talent)
        {
            return talents[talent].Cast(evoker);
        }

        public CommandResult Evoke(Entity evoker, int talent,
            Cell cell, List<Cell> line, List<Cell> path)
        {
            return talents[talent].Cast(evoker, cell, line, path);
        }

        public void AddTalent(Talent talent)
        {
            Array.Resize(ref talents, talents.Length + 1);
            talents[talents.GetUpperBound(0)] = talent;
        }

        public override EntityComponent Clone(bool full)
        {
            return new Evocable(EvokeTime, Talents);
        }
    }
}
