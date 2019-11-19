// Body.cs
// Jerome Martina

using System.Collections.Generic;

namespace Pantheon.ECS.Components
{
    [System.Serializable]
    public sealed class Body : BaseComponent
    {
        [UnityEngine.SerializeField] private List<Entity> parts
            = new List<Entity>();
        public List<Entity> Parts => parts;

        public override BaseComponent Clone()
        {
            throw new System.NotImplementedException();
        }
    }
}
