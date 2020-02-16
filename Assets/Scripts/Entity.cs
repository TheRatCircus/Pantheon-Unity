// Entity.cs
// Jerome Martina

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Pantheon.Components.Entity;
using Pantheon.Content;
using Pantheon.Core;
using Pantheon.Utils;
using Pantheon.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

namespace Pantheon
{
    [Flags]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EntityFlag : byte
    {
        Blocking = 1 << 0,
        InInventory = 1 << 1,
        Wielded = 1 << 2,
        Unique = 1 << 3,
        Male = 1 << 4,
        Female = 1 << 5
    }

    [Serializable]
    public sealed class Entity
    {
        public string Name { get; set; }
        public EntityFlag Flags { get; set; }

        public Vector2Int Cell { get; set; }
        public Level Level { get; set; }

        public bool Blocking
        {
            get => Flags.HasFlag(EntityFlag.Blocking);
            set
            {
                if (value)
                    Flags |= EntityFlag.Blocking;
                else
                    Flags &= ~EntityFlag.Blocking;
            }
        }
        public bool InInventory
        {
            get => Flags.HasFlag(EntityFlag.InInventory);
            set
            {
                if (value)
                    Flags |= EntityFlag.InInventory;
                else
                    Flags &= ~EntityFlag.InInventory;
            }
        }
        public bool Wielded
        {
            get => Flags.HasFlag(EntityFlag.Wielded);
            set
            {
                if (value)
                    Flags |= EntityFlag.Wielded;
                else
                    Flags &= ~EntityFlag.Wielded;
            }
        }
        public bool Unique
        {
            get => Flags.HasFlag(EntityFlag.Unique);
            set
            {
                if (value)
                    Flags |= EntityFlag.Unique;
                else
                    Flags &= ~EntityFlag.Unique;
            }
        }

        public bool Visible => Level.Visible(Cell.x, Cell.y);

        // TODO: Reduce to one GameObject reference
        [NonSerialized]
        private GameObject[] gameObjects = new GameObject[1];
        public GameObject[] GameObjects
        {
            get => gameObjects;
            set => gameObjects = value;
        }

        public Sprite Sprite => Flyweight.Sprite;
        public Tile Tile => Flyweight.Tile;
        public EntityTemplate Flyweight { get; set; }

        private EntityComponent[] components;
        public EntityComponent[] Components
        {
            get => components;
            private set => components = value;
        }

        public event Action BecameVisibleEvent;
        public event Action DestroyedEvent;

        public Entity(params EntityComponent[] components)
        {
            Components = new EntityComponent[components.Length];

            foreach (EntityComponent ec in components)
                AddComponent(ec.Clone(false));
        }

        public Entity(EntityTemplate template)
        {
            Name = template.EntityName;
            Flyweight = template;
            Flags = template.Flags;

            if (template.Components == null || template.Components.Length < 1)
            {
                Components = new EntityComponent[0];
                return;
            }

            Components = new EntityComponent[template.Components.Length];

            foreach (EntityComponent component in template.Components)
            {
                if (component is IEntityDependentComponent)
                    AddComponent(component.Clone(false));

                if (component is Wield w && template.Wielded?.Length > 0)
                {
                    AddComponent(w.Clone(false));
                    Wield wield = GetComponent<Wield>();
                    for (int i = 0; i < template.Wielded.Length; i++)
                        wield.TryWield(new Entity(template.Wielded[i]), out Entity unwielded);
                }
            }
        }

        public T GetComponent<T>() where T : EntityComponent
        {
            foreach (EntityComponent ec in Components)
            {
                if (ec?.GetType() == typeof(T))
                    return (T)ec;
            }

            if (Flyweight != null && Flyweight.TryGetComponent(out T tec))
            {
                // Get clone from template, add to Components, return it
                T clone = (T)tec.Clone(false);
                AddComponent(clone);
                return clone;
            }
            else return null;
        }

        public bool TryGetComponent<T>(out T ret)
            where T : EntityComponent
        {
            foreach (EntityComponent ec in Components)
            {
                if (ec?.GetType() == typeof(T))
                {
                    ret = (T)ec;
                    return true;
                }
            }

            if (Flyweight != null && Flyweight.TryGetComponent(out T tec))
            {
                // Get clone from template, add to Components, return it
                T clone = (T)tec.Clone(false);
                AddComponent(clone);
                ret = clone;
                return true;
            }
            else
            {
                ret = null;
                return false;
            }
        }

        public void AddComponent(EntityComponent ec)
        {
            ec.Entity = this;
            if (ec is IEntityDependentComponent edc)
                edc.Initialize();
            ec.MessageEvent += RelayMessage;

            Type type = ec.GetType();

            // First check for existing component of same type
            for (int i = 0; i < Components.Length; i++)
                if (Components[i]?.GetType() == type)
                    throw new ArgumentException(
                        $"Component of type {type.Name} already present in entity {Name}.");

            for (int i = 0; i < Components.Length; i++)
                if (Components[i] == null)
                {
                    Components[i] = ec;
                    return;
                }

            Array.Resize(ref components, Components.Length + 1);
            Components[Components.Length - 1] = ec;
        }

        public bool HasComponent<T>() where T : EntityComponent
        {
            foreach (EntityComponent ec in Components)
                if (ec.GetType() == typeof(T))
                    return true;

            return false;
        }

        public bool HasComponent(Type type)
        {
            if (type.BaseType != typeof(EntityComponent))
                throw new ArgumentException(
                    $"Type must inherit from EntityComponent.");

            foreach (EntityComponent ec in Components)
                if (ec.GetType() == type)
                    return true;

            return false;
        }

        private void RelayMessage(IComponentMessage msg)
        {
            foreach (EntityComponent ec in Components)
                ec?.Receive(msg);
        }

        public void Move(Level level, Vector2Int cell)
        {
            Locator.Scheduler.SetDirtyCell(Cell);
            Locator.Scheduler.SetDirtyCell(cell);
            Vector2Int prev = Cell;
            Level = level;
            Cell = cell;
            level.MoveEntity(this, prev, Cell);

            if (GameObjects.HasElements())
                GameObjects[0].transform.position = Cell.ToVector3();

            if (TryGetComponent(out Inventory inv))
                inv.Move(level, cell);
            if (TryGetComponent(out Wield wield))
                wield.Move(level, cell);
        }

        public void MakeVisible() => BecameVisibleEvent?.Invoke();

        public void TakeHit(Entity hitter, Hit hit)
        {
            // TODO: OnHitEvent
            if (TryGetComponent(out Splat splat))
            {
                if (splat.Sound != null)
                    Locator.Audio.Buffer(splat.Sound, Cell.ToVector3());
                if (splat.FXPrefab != null && RandomUtils.OneChanceIn(2))
                    Object.Destroy(Object.Instantiate(
                        splat.FXPrefab, Cell.ToVector3(),
                        new Quaternion(), null) as GameObject, 1f);
            }

            if (TryGetComponent(out Health health))
            {
                foreach (HitDamage damage in hit.damages)
                    if (health.Damage(hitter, damage))
                    {
                        Destroy(hitter);
                        break;
                    }
            }
        }

        public void Destroy(Entity destroyer)
        {
            DestroyedEvent?.Invoke();

            if (TryGetComponent(out Species species))
            {
                Entity corpse = new Entity(Assets.GetCorpseTemplate(this));
                corpse.Move(Level, Cell);
            }

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
                    Locator.Log.Send(Verbs.Kill(destroyer, this), Color.white);
                }
                else
                    Locator.Log.Send(
                        $"{Strings.Subject(this, true)} is destroyed.",
                        Color.grey);
            }

            Level.ClearEntity(this);
            Object.Destroy(GameObjects[0]);
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext ctxt)
        {
            DestroyedEvent = null;
        }

        public override string ToString()
        {
            if (TryGetComponent(out Relic relic))
                return relic.Name;
            else
                return Name;
        }
    }
}
