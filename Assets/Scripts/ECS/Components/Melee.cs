// Melee.cs
// Jerome Martina

using System;

namespace Pantheon.ECS.Components
{
    /// <summary>
    /// Allows an entity to be used as a melee weapon.
    /// </summary>
    [Serializable]
    public sealed class Melee : BaseComponent
    {
        [Newtonsoft.Json.JsonProperty] private Attack[] attacks;

        public override BaseComponent Clone()
        {
            throw new NotImplementedException();
        }
    }
}
