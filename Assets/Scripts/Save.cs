// Save.cs
// Jerome Martina

using Pantheon.Core;
using Pantheon.Actors;
using Pantheon.World;
using Pantheon.WorldGen;
using System.Collections.Generic;

namespace Pantheon
{
    [System.Serializable]
    public sealed class Save
    {
        public string SaveName { get; set; }
        public int Seed { get; set; }
        public Core.Pantheon Pantheon { get; set; }
        //public List<Actor> Queue { get; set; }
        //public Actor CurrentActor { get; set; }
        public int TurnProgress { get; set; }
        public int Turns { get; set; }
        //public Player Player { get; set; }
        //public Level ActiveLevel { get; set; }
        public Dictionary<int, Layer> Layers { get; set; }
        //public Dictionary<string, Level> Levels { get; set; }
        public Dictionary<string, LevelBuilder> BuilderMap { get; set; }
        public Faction Nature { get; set; }
        public Dictionary<Idol, Faction> Religions { get; set; }

        public bool IdolMode { get; set; }
    }

    public class ActorSave
    {
        public Level Level { get; set; }
        public Cell Cell { get; set; }

        public string ActorName { get; set; }
        public bool IsUnique { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int RegenRate { get; set; }
        public int RegenProgress { get; set; }
        public int Speed { get; set; }
        public int Energy { get; set; }

        public int XP { get; set; }
        private int XPValue { get; set; }
        public int ExpLevel { get; private set; } = 1;

        public Body Body { get; set; }
        public Defenses Defenses { get; set; }
        public List<Trait> Traits { get; set; }
            = new List<Trait>();
        public List<StatusEffect> Statuses { get; set; }
            = new List<StatusEffect>();
        public Equipment Equipment { get; set; }
        public Inventory Inventory { get; set; }
        public List<Spell> Spells { get; set; } = new List<Spell>();
        public Faction Faction { get; set; }

        public ActorSave(Actor actor)
        {
            Level = actor.level;
            Cell = actor.Cell;
            ActorName = actor.ActorName;
            IsUnique = actor.IsUnique;
            Health = actor.Health;
            MaxHealth = actor.MaxHealth;
            RegenRate = actor.RegenRate;
            RegenProgress = actor.RegenProgress;
            Speed = actor.Speed;
            Energy = actor.Energy;
            XP = actor.XP;
            XPValue = actor.XPValue;
            ExpLevel = actor.ExpLevel;
            Body = actor.Body;
            Defenses = actor.Defenses;
            Traits = actor.Traits;
            Statuses = actor.Statuses;
            Equipment = actor.Equipment;
            Inventory = actor.Inventory;
            Spells = actor.Spells;
            Faction = actor.Faction;
        }
    }
}
