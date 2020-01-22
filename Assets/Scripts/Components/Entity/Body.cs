// Body.cs
// Jerome Martina

using Pantheon.Content;
using System.Collections.Generic;

namespace Pantheon.Components.Entity
{
    [System.Serializable]
    public sealed class Body : EntityComponent
    {
        public BodyPart Torso { get; set; }
        public List<BodyPart> Parts { get; set; }

        public override EntityComponent Clone(bool full)
        {
            return new Body()
            {
                Torso = Torso,
                Parts = new List<BodyPart>(Parts)
            };
        }
    }
}
