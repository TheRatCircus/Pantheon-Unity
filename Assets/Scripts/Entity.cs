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

        [NonSerialized]
        private GameObject[] gameObjects = new GameObject[1];
        public GameObject[] GameObjects
        {
            get => gameObjects;
            set => gameObjects = value;
        }

        private Sprite sprite;
        public Sprite Sprite
        {
            get
            {
                if (sprite != null)
                    return sprite;
                else
                    return Flyweight.Sprite;
            }
            set => sprite = value;
        }
        [NonSerialized] private Tile tile;
        public Tile Tile
        {
            get
            {
                if (tile != null)
                    return tile;
                else
                    return Flyweight.Tile;
            }
            set => tile = value;
        }
        public EntityTemplate Flyweight { get; set; }

        public Dictionary<Type, EntityComponent> Components { get; private set; }

        public event Action DestroyedEvent;

        public Entity(params EntityComponent[] components)
        {
            Components = new Dictionary<Type, EntityComponent>();

            foreach (EntityComponent ec in components)
                AddComponent(ec.Clone(false));

            ConnectComponents();
        }

        public Entity(EntityTemplate template)
        {
            Name = template.EntityName;
            Flyweight = template;
            Flags = template.Flags;
            Components = new Dictionary<Type, EntityComponent>(5);

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
            ConnectComponents();
        }

        private void ConnectComponents()
        {
            if (TryGetComponent(out AI ai))
                ai.Actor = GetComponent<Actor>();
        }

        public T GetComponent<T>() where T : EntityComponent
        {
            if (Components.TryGetValue(typeof(T), out EntityComponent ret))
                return (T)ret;
            else if (Flyweight != null && Flyweight.TryGetComponent(out T tec))
            {
                // Get clone from template, add to Components, return it
                T clone = (T)tec.Clone(false);
                AddComponent(clone);
                return clone;
            }
            else
                throw new ArgumentException(
                    $"Component of type {typeof(T)} not found.");
        }

        public bool TryGetComponent<T>(out T ret)
            where T : EntityComponent
        {
            if (Components.TryGetValue(typeof(T), out EntityComponent ec))
            {
                ret = (T)ec;
                return true;
            }
            else if (Flyweight != null && Flyweight.TryGetComponent(out T tec))
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
            Components.Add(ec.GetType(), ec);
        }

        public bool HasComponent<T>() where T : EntityComponent
        {
            return Components.ContainsKey(typeof(T));
        }

        public bool HasComponent(Type type)
        {
            if (type.BaseType != typeof(EntityComponent))
                throw new ArgumentException(
                    $"Type must inherit from EntityComponent.");

            return Components.ContainsKey(type);
        }

        private void RelayMessage(IComponentMessage msg)
        {
            foreach (EntityComponent ec in Components.Values)
                ec.Receive(msg);
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
                Tile tile = ScriptableObject.CreateInstance<Tile>();
                Sprite sprite = Assets.Sprites["Sprite_Corpse"];
                tile.sprite = sprite;
                Entity corpse = new Entity(new Corpse())
                {
                    Name = $"{species.SpeciesDef.Name} corpse",
                    Sprite = sprite,
                    Tile = tile
                };
                corpse.Move(Level, Cell);
                corpse.GetComponent<Corpse>().Original = this;
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

        [OnDeserializing]
        private void OnDeserializing(StreamingContext ctxt)
        {
            Tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = sprite;
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
