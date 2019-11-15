// UnityGameObject.cs
// Jerome Martina

using System;
using UnityEngine;

namespace Pantheon.ECS.Components
{
    /// <summary>
    /// Holds a reference to a Unity GameObject representing an entity.
    /// </summary>
    [Serializable]
    public sealed class UnityGameObject : BaseComponent
    {
        [NonSerialized] private GameObject gameObject;
        public GameObject GameObject { get => gameObject; set => gameObject = value; }

        public override BaseComponent Clone()
        {
            return new UnityGameObject();
        }
    }
}
