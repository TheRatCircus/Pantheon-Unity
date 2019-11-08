// Entity.cs
// Jerome Martina

using Pantheon.ECS.Components;
using Pantheon.ECS.Templates;
using Pantheon.ECS.Messages;
using System;
using System.Collections.Generic;

namespace Pantheon.ECS
{
    [Serializable]
    public sealed class Entity
    {
        [UnityEngine.SerializeField]
        //[ReadOnly]
        private Dictionary<Type, BaseComponent> components
            = new Dictionary<Type, BaseComponent>();
        public Dictionary<Type, BaseComponent> Components => components;

        public Entity(params BaseComponent[] components)
        {
            foreach (BaseComponent c in components)
                Components.Add(c.GetType(), c);
        }

        public Entity(Template template)
        {
            foreach (BaseComponent c in template.Unload())
                Components.Add(c.GetType(), c); 
        }

        public T GetComponent<T>() where T : BaseComponent
        {
            if (!components.TryGetValue(typeof(T), out BaseComponent ret))
                throw new ArgumentException(
                    $"Component of type {typeof(T)} not found.");
            else
                return (T)ret;
        }

        public bool HasComponent<T>() where T : BaseComponent
        {
            return components.ContainsKey(typeof(T));
        }

        public void AddComponent(BaseComponent component)
        {
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
    }
}
