// Entity.cs
// Jerome Martina

using System;
using UnityEngine;

namespace Pantheon.ECS
{
    [Serializable]
    public sealed class Entity
    {
        public string Name { get; set; }
        public int GUID { get; set; }

        public bool Blocking { get; set; }
        [NonSerialized]
        private GameObject[] gameObjects = new GameObject[1];
        public GameObject[] GameObjects
        {
            get => gameObjects;
            set => gameObjects = value;
        }

        public EntityTemplate Flyweight { get; set; }

        public Entity(EntityTemplate template)
        {
            Name = template.EntityName;
            Flyweight = template;
        }

        public override string ToString() => $"{Name} ({GUID})";
    }
}
