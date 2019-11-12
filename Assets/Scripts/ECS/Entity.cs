// Entity.cs
// Jerome Martina

using Pantheon.ECS.Components;
using Pantheon.ECS.Messages;
using Pantheon.ECS.Templates;
using System;
using System.Collections.Generic;

namespace Pantheon.ECS
{
    public enum EntityArchetype
    {
        None,
        Player,
        NPC,
        Ground,
        Wall,
        Item
    }

    [Serializable]
    public sealed class Entity
    {
        public string Name { get; private set; } = "DEFAULT_ENTITY_NAME";
        public int GUID { get; private set; }
        public EntityArchetype Archetype { get; private set; }
            = EntityArchetype.None;

        [UnityEngine.SerializeField]
        private Dictionary<Type, BaseComponent> components
            = new Dictionary<Type, BaseComponent>();
        public Dictionary<Type, BaseComponent> Components => components;

        public Entity(string name, params BaseComponent[] components)
        {
            Name = name;
            foreach (BaseComponent c in components)
                AddComponent(c);
            ConnectComponents();
        }

        public Entity(string name, Template template)
        {
            Name = name;
            Archetype = template.Archetype;
            foreach (BaseComponent c in template.Unload())
                AddComponent(c.Clone());
            ConnectComponents();
        }

        // Some components are interdependent,
        // so hook them up after construction
        private void ConnectComponents()
        {
            if (components.TryGetValue(typeof(Player), out BaseComponent c))
            {
                Player p = (Player)c;
                p.Entity = this;
                p.Actor = GetComponent<Actor>();
            }
        }

        public void SetGUID(int guid)
        {
            GUID = guid;
            foreach (BaseComponent c in components.Values)
                c.AssignToEntity(this);
        }

        public T GetComponent<T>() where T : BaseComponent
        {
            if (components.TryGetValue(typeof(T), out BaseComponent ret))
                return (T)ret;
            else
                throw new ArgumentException(
                    $"Component of type {typeof(T)} not found.");
        }

        public bool TryGetComponent<T>(out T ret)
            where T : BaseComponent
        {
            if (!components.TryGetValue(typeof(T), out BaseComponent c))
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

        public bool HasComponent<T>() where T : BaseComponent
            => components.ContainsKey(typeof(T));

        public void AddComponent(BaseComponent component)
        {
            component.AssignToEntity(this);
            component.MessageEvent += Message;
            components.Add(component.GetType(), component);
        }

        public void RemoveComponent<T>() where T : BaseComponent
        {
            components.TryGetValue(typeof(T), out BaseComponent c);
            c.MessageEvent -= Message;
            components.Remove(typeof(T));
        }

        private void Message(ComponentMessage msg)
        {
            if (!components.TryGetValue(msg.Target, out BaseComponent target))
                throw new Exception(
                    $"Component of type {msg.Target} not found.");
            else
                target.Receive(msg);
        }

        public void SetEnabled(bool enabled)
        {
            foreach (BaseComponent c in components.Values)
                c.Enabled = enabled;
        }

        public string ListComponents()
        {
            string ret = "";
            foreach (BaseComponent c in components.Values)
                ret += c;
            return ret;
        }

        public override string ToString() => Name;
    }
}
