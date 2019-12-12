// EntityWrapper.cs
// Jerome Martina

using Pantheon.ECS;
using UnityEngine;

namespace Pantheon
{
    /// <summary>
    /// Allows a GameObject to reference its entity.
    /// </summary>
    public sealed class EntityWrapper : MonoBehaviour
    {
        public Entity Entity { get; set; }
    }
}
