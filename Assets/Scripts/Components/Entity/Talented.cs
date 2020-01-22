// Talented.cs
// Jerome Martina

using Pantheon.Commands;
using Pantheon.World;
using System;

namespace Pantheon.Components.Entity
{
    using Entity = Pantheon.Entity;
    using Talent = Pantheon.Talent;

    [Serializable]
    public sealed class Talented : EntityComponent
    {
        public Talent[] Talents { get; set; }

        public CommandResult Evoke(Entity caster, int talent, Cell target)
        {
            return Talents[talent].Cast(caster, target);
        }

        public override EntityComponent Clone(bool full)
        {
            return new Talented() { Talents = Talents };
        }
    }
}
