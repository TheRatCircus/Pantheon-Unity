// Entity.cs
// Jerome Martina

using Pantheon.Components;
using Pantheon.Core;
using Pantheon.UI;
using Pantheon.Util;
using Pantheon.Utils;
using Pantheon.World;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon
{
    [Serializable]
    public sealed class Entity
    {
        public string Name { get; set; }

        public bool Blocking { get; set; } = false;
        [NonSerialized]
        private GameObject[] gameObjects = new GameObject[1];
        public GameObject[] GameObjects
        {
            get => gameObjects;
            set => gameObjects = value;
        }

        public EntityTemplate Flyweight { get; set; }

        private Dictionary<Type, EntityComponent> components;

        public Cell Cell
        {
            get => GetComponent<Location>().Cell;
            set => GetComponent<Location>().Cell = value;
        }
        public Level Level
        {
            get => GetComponent<Location>().Level;
            set => GetComponent<Location>().Level = value;
        }
        // Address in third-person if not player entity
        public bool ThirdPerson
        {
            get
            {
                if (TryGetComponent(out Actor actor))
                    return actor.Control != ActorControl.Player;
                else
                    return true;
            }
        }
        public bool PlayerControlled
        {
            get
            {
                if (TryGetComponent(out Actor actor))
                    return actor.Control == ActorControl.Player;
                else
                    return false;
            }
        }

        public event Action DestroyedEvent;

        /// <summary>
        /// Construct a temporary pretend entity from a TerrainDef.
        /// </summary>
        /// <param name="terrain"></param>
        /// <returns></returns>
        public static Entity VirtualEntity(TerrainDefinition terrain)
        {
            return new Entity(terrain.DisplayName);
        }

        public Entity(string name)
        {
            Name = name;
            components = new Dictionary<Type, EntityComponent>();
            AddComponent(new Location());
            ConnectComponents();
        }

        public Entity(params EntityComponent[] components)
        {
            this.components = new Dictionary<Type, EntityComponent>();

            foreach (EntityComponent ec in components)
                AddComponent(ec.Clone());

            AddComponent(new Location());
            ConnectComponents();
        }

        public Entity(EntityTemplate template)
        {
            Name = template.EntityName;
            Flyweight = template;

            components = new Dictionary<Type, EntityComponent>();

            foreach (EntityComponent component in template.Components)
                AddComponent(component.Clone());

            AddComponent(new Location());
            ConnectComponents();
        }

        private void ConnectComponents()
        {
            if (TryGetComponent(out AI ai))
            {
                ai.Entity = this;
                Actor actor = GetComponent<Actor>();
                ai.SetActor(actor);
            }
        }

        public T GetComponent<T>() where T : EntityComponent
        {
            if (components.TryGetValue(typeof(T), out EntityComponent ret))
                return (T)ret;
            else
                throw new ArgumentException(
                    $"Component of type {typeof(T)} not found.");
        }

        public bool TryGetComponent<T>(out T ret)
            where T : EntityComponent
        {
            if (!components.TryGetValue(typeof(T), out EntityComponent c))
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

        public void AddComponent(EntityComponent ec)
        {
            ec.MessageEvent += RelayMessage;
            components.Add(ec.GetType(), ec);
        }

        public bool HasComponent<T>() where T : EntityComponent
        {
            return components.ContainsKey(typeof(T));
        }

        private void RelayMessage(IComponentMessage msg)
        {
            foreach (EntityComponent ec in components.Values)
                ec.Receive(msg);
        }

        public void Move(Level level, Cell cell)
        {
            Cell prev = Cell;

            if (prev != null)
                prev.DeallocateEntity(this);

            Level = level;
            Cell = cell;
            if (GameObjects.HasElements())
                GameObjects[0].transform.position = cell.Position.ToVector3();

            if (TryGetComponent(out Inventory inv))
                inv.Move(level, cell);

            Cell.AllocateEntity(this);
        }

        public void TakeHit(Entity hitter, Hit hit)
        {
            // TODO: OnHitEvent
            Health health = GetComponent<Health>();
            foreach (HitDamage damage in hit.damages)
                if (health.Damage(damage))
                {
                    Destroy(hitter);
                    break;
                }
        }

        public void Destroy(Entity destroyer)
        {
            DestroyedEvent?.Invoke();

            if (TryGetComponent(out Actor actor))
            {
                SchedulerLocator._scheduler.RemoveActor(actor);

                if (actor.Control == ActorControl.Player)
                {
                    Transform camTransform = GameObjects[0].transform.Find("MainCamera");
                    camTransform.SetParent(null);
                    SchedulerLocator._scheduler.Lock();
                    LogLocator._log.Send($"You perish...", Color.magenta);
                }
                else if (actor.Control == ActorControl.AI)
                {
                    LogLocator._log.Send(Strings.Kill(destroyer, this), Color.white);
                }
                else
                    LogLocator._log.Send(
                        $"{ToSubjectString(true)} is destroyed.",
                        Color.grey);
            }

            Cell.DeallocateEntity(this);
            Cell = null;
            Level = null;
            components = null;

            UnityEngine.Object.Destroy(GameObjects[0]);
        }

        public string ToSubjectString(bool sentenceStart)
        {
            Actor a = GetComponent<Actor>();

            if (a.Control == ActorControl.Player)
                return sentenceStart ? "You" : "you";
            else
                return sentenceStart ? $"The {Name}" : $"the {Name}";
        }

        public override string ToString() => Name;
    }
}
