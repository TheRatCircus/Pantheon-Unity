// Template.cs
// Jerome Martina

using UnityEngine;
using Pantheon.ECS.Components;

namespace Pantheon.ECS.Templates
{
    public abstract class Template : ScriptableObject
    {
        public abstract BaseComponent[] Unload();
    }
}
