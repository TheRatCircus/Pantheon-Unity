// Evocable.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon.Commands;
using Pantheon.Content.Talents;
using Pantheon.Utils;
using System;
using UnityEngine;

namespace Pantheon.Components
{
    [Serializable]
    public sealed class Talents : EntityComponent
    {
        public Talent[] All { get; set; }

        [JsonConstructor]
        public Talents(params Talent[] talents) => All = talents;

        public CommandResult Evoke(Entity evoker, int talent)
        {
            return All[talent].Cast(evoker);
        }

        public CommandResult Evoke(Entity evoker, int talent,
            Vector2Int cell, Line line, Line path)
        {
            return All[talent].Cast(evoker);
        }

        public override EntityComponent Clone(bool full)
        {
            return new Talents(All);
        }
    }
}
