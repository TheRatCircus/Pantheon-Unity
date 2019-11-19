// Entity.cs
// Jerome Martina

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Pantheon.ECS.Components;
using Pantheon.ECS.Messages;
using Pantheon.ECS.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pantheon.ECS
{
    [JsonConverter(typeof(StringEnumConverter))]
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
        public int GUID { get; private set; } = -1;

        [NonSerialized]
        private Template flyweight = null;
        public Template Flyweight { get => flyweight; set => flyweight = value; }
        public string FlyweightID { get; private set; } = "DEFAULT_FLYWEIGHT_ID";

        public EntityArchetype Archetype { get; private set; }
            = EntityArchetype.None;
        public bool HasGameObject => HasComponent<Actor>();

        public Dictionary<Type, BaseComponent> Components { get; private set; }
            = null;
        public bool FlyweightOnly => Components == null;

        public Entity(string name, params BaseComponent[] components)
        {
            Name = name;
            // TODO: Based on composition, resolve
            // archetype and find a flyweight
            Components = new Dictionary<Type, BaseComponent>();
            foreach (BaseComponent c in components)
                AddComponent(c);
            ConnectComponents();
        }

        /// <summary>
        /// Construct a new entity from a template.
        /// </summary>
        /// <param name="template"></param>
        /// <param name="flyweightOnly">Is this entity only a reference to a flyweight?</param>
        public Entity(Template template, bool flyweightOnly)
        {
            Name = template.EntityName;
            Flyweight = template;
            FlyweightID = Flyweight.name;
            Archetype = template.Archetype;
            
            if (!flyweightOnly)
            {
                Components = new Dictionary<Type, BaseComponent>();
                foreach (BaseComponent c in template.Unload())
                    AddComponent(c.Clone());

                ConnectComponents();
            }
        }

        // Some components are interdependent,
        // so hook them up after construction
        private void ConnectComponents()
        {
            if (Components.TryGetValue(typeof(Player), out BaseComponent c))
            {
                Player p = (Player)c;
                p.Entity = this;
                p.Actor = GetComponent<Actor>();
            }
        }

        public void SetGUID(int guid)
        {
            GUID = guid;
            foreach (BaseComponent c in Components.Values)
                c.AssignToEntity(this);
        }

        public T GetComponent<T>() where T : BaseComponent
        {
            if (Components.TryGetValue(typeof(T), out BaseComponent ret))
                return (T)ret;
            else
                throw new ArgumentException(
                    $"Component of type {typeof(T)} not found.");
        }

        public bool TryGetComponent<T>(out T ret)
            where T : BaseComponent
        {
            if (Components == null)
            {
                ret = Flyweight.Unload().OfType<T>().SingleOrDefault();
                return Flyweight.Unload().OfType<T>().Any();
            }
            else if (!Components.TryGetValue(typeof(T), out BaseComponent c))
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
        {
            if (Components == null)
                return Flyweight.Unload().OfType<T>().Any();
            else
                return Components.ContainsKey(typeof(T));
        }

        public void AddComponent(BaseComponent component)
        {
            component.AssignToEntity(this);
            component.MessageEvent += Message;
            Components.Add(component.GetType(), component);
        }

        public void RemoveComponent<T>() where T : BaseComponent
        {
            Components.TryGetValue(typeof(T), out BaseComponent c);
            c.MessageEvent -= Message;
            Components.Remove(typeof(T));
        }

        private void Message(ComponentMessage msg)
        {
            if (!Components.TryGetValue(msg.Target, out BaseComponent target))
                throw new Exception(
                    $"Component of type {msg.Target} not found.");
            else
                target.Receive(msg);
        }

        public void SetEnabled(bool enabled)
        {
            foreach (BaseComponent c in Components.Values)
                c.Enabled = enabled;
        }

        public string ListComponents()
        {
            string ret = "";
            foreach (BaseComponent c in Components.Values)
                ret += c;
            return ret;
        }

        public override string ToString() => Name;
    }
}
