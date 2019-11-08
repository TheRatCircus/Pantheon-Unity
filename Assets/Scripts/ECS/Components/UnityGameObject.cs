// UnityGameObject.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon.ECS.Components
{
    /// <summary>
    /// Holds a reference to a Unity GameObject representing an entity.
    /// </summary>
    [System.Serializable]
    public sealed class UnityGameObject : BaseComponent
    {
        public GameObject GameObject { get; private set; }
    }
}
