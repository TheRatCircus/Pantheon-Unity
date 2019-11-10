// Template.cs
// Jerome Martina

using UnityEngine;
using Pantheon.ECS.Components;

namespace Pantheon.ECS.Templates
{
    public abstract class Template : ScriptableObject
    {
        [SerializeField] private string entityName = "DEFAULT_ENTITY_NAME";
        public string Name => entityName;
        public abstract BaseComponent[] Unload();
    }
}
