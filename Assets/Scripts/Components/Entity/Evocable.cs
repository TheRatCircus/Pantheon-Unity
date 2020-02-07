// Evocable.cs
// Jerome Martina

using Pantheon.Commands;
using Pantheon.Components.Talent;
using Pantheon.World;
using System;

namespace Pantheon.Components.Entity
{
    using Talent = Pantheon.Talent;
    using Entity = Pantheon.Entity;

    [Serializable]
    public sealed class Evocable : EntityComponent, IEntityDependentComponent
    {
        private Talent[] talents;
        public Talent[] Talents { get => talents; set => talents = value; }

        public Evocable(params Talent[] talents) => this.talents = talents;

        public void SetEntity()
        {
            foreach (Talent t in talents)
                if (t.Behaviour is IEntityBasedTalent ebt)
                    ebt.Entity = Entity;
        }

        public CommandResult Evoke(Entity caster, int talent, Cell target)
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
