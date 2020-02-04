// Entity.cs
// Jerome Martina

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Pantheon.Components.Entity;
using Pantheon.Content;
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
        Unique = 1 << 2
    }

    [Serializable]
    public sealed class Entity
    {
        public string Name { get; set; }
        public EntityFlag Flags { get; set; }

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

        public event Action DestroyedEvent;

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

            //foreach (EntityComponent component in template.Components)
            //    AddComponent(component.Clone(false));

            AddComponent(new Location());
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
            {
                prev.DeallocateEntity(this);
                Locator.Scheduler.MarkDirtyCell(prev);
            }

            Level = level;
            Cell = cell;
            if (GameObjects.HasElements())
                GameObjects[0].transform.position = cell.Position.ToVector3();

            if (TryGetComponent(out Inventory inv))
                inv.Move(level, cell);

            Cell.AllocateEntity(this);
            Locator.Scheduler.MarkDirtyCell(Cell);
        }

        public void TakeHit(Entity hitter, Hit hit)
        {
            // TODO: OnHitEvent
            if (TryGetComponent(out Splat splat))
            {
                if (splat.Sound != null)
                    Locator.Audio.Buffer(splat.Sound, Cell.Position.ToVector3());
                if (splat.FXPrefab != null && RandomUtils.OneChanceIn(2))
                    Object.Destroy(Object.Instantiate(
                        splat.FXPrefab, Cell.Position.ToVector3(),
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
                        $"{ToSubjectString(true)} is destroyed.",
                        Color.grey);
            }

            Cell.DeallocateEntity(this);
            UnityEngine.Object.Destroy(GameObjects[0]);
        }

        public string ToSubjectString(bool sentenceStart)
        {
            if (TryGetComponent(out Relic relic))
                return relic.Name;
            else if (TryGetComponent(out Actor actor))
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
