// Talented.cs
// Jerome Martina

using Pantheon.Commands;
using System;
using UnityEngine;

namespace Pantheon.Components.Entity
{
    using Entity = Pantheon.Entity;

    [Serializable]
    public sealed class Talented : EntityComponent
    {
        public Talent[] Talents { get; set; }

        public CommandResult Evoke(Entity caster, int talent, Vector2Int target)
        {
            return Talents[talent].Cast(caster, target);
        }

        public override EntityComponent Clone(bool full)
        {
            return new Talented() { Talents = Talents };
        }
    }
}
