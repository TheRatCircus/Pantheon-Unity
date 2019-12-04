// BaseComponent.cs
// Jerome Martina

using Newtonsoft.Json;
using System;

namespace Pantheon.ECS.Components
{
    //[JsonConverter(typeof(ComponentConverter))]
    public abstract class BaseComponent
    {
        [JsonIgnore] public int GUID { get; private set; } = -1;
        [JsonIgnore] public bool Enabled { get; set; } // Do systems update this component?

        public void AssignToEntity(Entity e) => GUID = e.GUID;

        public abstract BaseComponent Clone();
    }
}
