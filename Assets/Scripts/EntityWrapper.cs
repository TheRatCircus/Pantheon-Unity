// EntityWrapper.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon
{
    /// <summary>
    /// Allows a GameObject to reference its entity.
    /// </summary>
    public sealed class EntityWrapper : MonoBehaviour
    {
        [SerializeField] [ReadOnly] private Entity entity;
        public Entity Entity { get => entity; set => entity = value; }
    }
}
