// ComponentWrapper.cs
// Jerome Martina

using UnityEngine;

/// <summary>
/// Serializable ScriptableObject wrapper for a Component-class-derived object.
/// </summary>
namespace Pantheon.Components
{
    public abstract class ComponentWrapper : ScriptableObject
    {
        public abstract IComponent Get { get; }
    }
}
