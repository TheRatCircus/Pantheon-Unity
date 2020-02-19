// Evocable.cs
// Jerome Martina

using Pantheon.Commands;
using System;
using UnityEngine;

namespace Pantheon.Components.Entity
{
    using Entity = Pantheon.Entity;

    [Serializable]
    public sealed class Evocable : EntityComponent, IEntityDependentComponent
    {
        private Talent[] talents;
        public Talent[] Talents { get => talents; set => talents = value; }

        public Evocable(params Talent[] talents) => this.talents = talents;

        public void Initialize()
        {
            foreach (Talent t in talents)
                t.Entity = Entity;
        }

        public CommandResult Evoke(Entity caster, int talent, Vector2Int target)
        {
            return Talents[talent].Cast(caster, target);
        }

        public void AddTalent(Talent talent)
        {
            Array.Resize(ref talents, talents.Length + 1);
            talents[talents.GetUpperBound(0)] = talent;
        }

        public override EntityComponent Clone(bool full)
        {
            return new Evocable(Talents);
        }
    }
}
