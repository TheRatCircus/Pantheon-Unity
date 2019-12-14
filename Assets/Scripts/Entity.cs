// Entity.cs
// Jerome Martina

using Pantheon.Components;
using Pantheon.Core;
using Pantheon.UI;
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

        public bool Blocking { get; set; }
        [NonSerialized]
        private GameObject[] gameObjects = new GameObject[1];
        public GameObject[] GameObjects
        {
            get => gameObjects;
            set => gameObjects = value;
        }

        public EntityTemplate Flyweight { get; set; }

        public Dictionary<Type, EntityComponent> Components { get; private set; }
            = new Dictionary<Type, EntityComponent>();

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

        public Entity(string name) => Name = name;

        public Entity(params EntityComponent[] components)
        {
            foreach (EntityComponent ec in components)
                Components.Add(ec.GetType(), ec);
        }

        public Entity(EntityTemplate template)
        {
            Name = template.EntityName;
            Flyweight = template;
            foreach (EntityComponent component in template.Components)
                Components.Add(component.GetType(), component.Clone());

            Components.Add(typeof(Location), new Location());
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
            if (prev != null)
                prev.Actor = null;
            Level = level;
            Cell = cell;
            if (GameObjects.HasElements())
                GameObjects[0].transform.position = cell.Position.ToVector3();
            cell.Actor = this;
        }

        public void TakeHit(Entity hitter, Hit hit)
        {
            // TODO: OnHitEvent
            Health health = GetComponent<Health>();
            foreach (HitDamage damage in hit.damages)
                if (health.Damage(damage.amount))
                {
                    Destroy();
                    break;
                }
        }

        public void Destroy()
        {
            // TODO: OnDestroyEvent
            // ???
            Cell.Actor = null;
            if (TryGetComponent(out Actor actor))
            {
                SchedulerLocator._scheduler.RemoveActor(actor);

                if (actor.Control == ActorControl.Player)
                {
                    Transform camTransform = GameObjects[0].transform.Find("MainCamera");
                    camTransform.SetParent(null);
                    SchedulerLocator._scheduler.Lock();
                    LogLocator._log.Send($"You perish...", Color.magenta);
                    DestroyedEvent.Invoke();
                }
            }
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
