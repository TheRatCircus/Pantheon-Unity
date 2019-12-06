// Entity.cs
// Jerome Martina

using Pantheon.Components;
using Pantheon.Utils;
using Pantheon.World;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon
{
    public sealed class Entity
    {
        public string Name { get; set; }

        public bool Blocking { get; set; }
        public GameObject[] GameObjects { get; private set; }
            = new GameObject[1];

        public Dictionary<Type, EntityComponent> Components { get; private set; }
            = new Dictionary<Type, EntityComponent>();

        public Cell Cell { get; set; }
        public Level Level { get; set; }

        public Entity(EntityTemplate template)
        {
            Name = template.Name;
            foreach (EntityComponent component in template.Components)
                Components.Add(component.GetType(), component);
        }

        public T GetComponent<T>() where T : EntityComponent
        {
            if (Components.TryGetValue(typeof(T), out EntityComponent ret))
                return (T)ret;
            else
                throw new ArgumentException(
                    $"Component of type {typeof(T)} not found.");
        }

        public bool TryGetComponent<T>(out T ret)
            where T : EntityComponent
        {
            if (!Components.TryGetValue(typeof(T), out EntityComponent c))
            {
                ret = null;
                return false;
            }
            else
            {
                ret = (T)c;
                return true;
            }
        }

        public bool HasComponent<T>() where T : EntityComponent
        {
            return Components.ContainsKey(typeof(T));
        }

        public void Move(Level level, Cell cell)
        {
            Cell prev = Cell;
            prev.Actor = null;
            Level = level;
            Cell = cell;
            GameObjects[0].transform.position = cell.Position.ToVector3();
            cell.Actor = this;
        }
    }
}
