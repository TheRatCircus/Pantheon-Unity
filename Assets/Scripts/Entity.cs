// Entity.cs
// Jerome Martina

using Pantheon.Components;
using Pantheon.Content;
using Pantheon.Util;
using Pantheon.Utils;
using Pantheon.World;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace Pantheon
{
    [Serializable]
    public sealed class Entity
    {
        public string Name { get; set; }

        public bool Blocking { get; set; } = false;
        public bool InInventory { get; set; } = false;
        [NonSerialized]
        private GameObject[] gameObjects = new GameObject[1];
        public GameObject[] GameObjects
        {
            get => gameObjects;
            set => gameObjects = value;
        }

        public EntityTemplate Flyweight { get; set; }

        public Dictionary<Type, EntityComponent> Components { get; private set; }

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
            Components = new Dictionary<Type, EntityComponent>();
            AddComponent(new Location());
            ConnectComponents();
        }

        public Entity(params EntityComponent[] components)
        {
            Components = new Dictionary<Type, EntityComponent>();

            foreach (EntityComponent ec in components)
                AddComponent(ec.Clone(false));

            AddComponent(new Location());
            ConnectComponents();
        }

        public Entity(EntityTemplate template)
        {
            Name = template.EntityName;
            Flyweight = template;

            Components = new Dictionary<Type, EntityComponent>();

            foreach (EntityComponent component in template.Components)
                AddComponent(component.Clone(false));

            AddComponent(new Location());
            ConnectComponents();
        }

        private void ConnectComponents()
        {
            if (TryGetComponent(out AI ai))
            {
                Actor actor = GetComponent<Actor>();
                ai.SetActor(actor);
            }
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

        public void AddComponent(EntityComponent ec)
        {
            ec.GetEntityEvent += delegate { return this; };
            ec.MessageEvent += RelayMessage;
            Components.Add(ec.GetType(), ec);
        }

        public bool HasComponent<T>() where T : EntityComponent
        {
            return Components.ContainsKey(typeof(T));
        }

        private void RelayMessage(IComponentMessage msg)
        {
            foreach (EntityComponent ec in Components.Values)
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
                if (health.Damage(hitter, damage))
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
                Locator.Scheduler.RemoveActor(actor);
                
                if (actor.Control == ActorControl.Player)
                {
                    Transform camTransform = GameObjects[0].transform.Find("MainCamera");
                    camTransform.SetParent(null);
                    Locator.Scheduler.Lock();
                    Locator.Log.Send($"You perish...", Color.magenta);
                }
                else if (actor.Control == ActorControl.AI)
                {
                    Locator.Log.Send(Strings.Kill(destroyer, this), Color.white);
                }
                else
                    Locator.Log.Send(
                        $"{ToSubjectString(true)} is destroyed.",
                        Color.grey);
            }

            Cell.DeallocateEntity(this);
            UnityEngine.Object.Destroy(GameObjects[0]);
        }

        public string ToSubjectString(bool sentenceStart)
        {
            if (TryGetComponent(out Actor actor))
            {
                if (actor.Control == ActorControl.Player)
                    return sentenceStart ? "You" : "you";
                else
                    return sentenceStart ? $"The {Name}" : $"the {Name}";
            }
            else
                return sentenceStart ? $"The {Name}" : $"the {Name}";
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext ctxt)
        {
            DestroyedEvent = null;
        }

        public override string ToString() => Name;
    }
}
