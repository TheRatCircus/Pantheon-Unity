// Actor.cs
// Jerome Martina

using Pantheon.Actions;
using Pantheon.Core;
using Pantheon.Utils;
using Pantheon.World;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.Actors
{
    /// <summary>
    /// Any entity which acts with any degree of agency.
    /// </summary>
    public class Actor : MonoBehaviour
    {
        public const int DefaultSpeed = 100;
        public const int DefaultMoveSpeed = 100;
        public const int DefaultRegenRate = 100;

        protected SpriteRenderer spriteRenderer;

        // Locational
        public Level level;
        [SerializeField] protected Cell cell;

        // Actor's personal attributes
        [SerializeField] protected string actorName;
        [SerializeField] protected bool isUnique;
        [SerializeField] [ReadOnly] protected int health;
        [SerializeField] protected int maxHealth = -1;
        [SerializeField] protected int regenRate = -1; // Time to regen 1 HP
        [SerializeField] [ReadOnly] protected int regenProgress = 0;
        [SerializeField] protected int speed = -1; // Energy per turn
        [SerializeField] [ReadOnly] protected int energy; // Energy remaining
        [SerializeField] protected int moveSpeed; // Energy needed to walk one cell

        [SerializeField] protected Attributes attributes;
        [SerializeField] protected Body body;
        [SerializeField] protected List<Trait> traits;
        [SerializeField]
        [ReadOnly]
        protected List<StatusEffect> statuses
            = new List<StatusEffect>();
        [SerializeField] protected Inventory inventory;
        [SerializeField] protected List<Spell> spells = new List<Spell>();
        public Faction Faction { get; set; }

        // Per-actor-type data
        [SerializeField] protected Species species;
        [SerializeField] protected Sprite corpseSprite;

        public Actor Master { get; set; } // For thralls
        [SerializeField] [ReadOnly] protected BaseAction nextAction;

        #region Properties

        public string ActorName { get => actorName; set => actorName = value; }
        public bool IsUnique { get => isUnique; set => isUnique = value; }
        public int Health { get => health; }
        public int MaxHealth { get => maxHealth; set => maxHealth = value; }
        public int Speed { get => speed; set => speed = value; }
        public int Energy { get => energy; set => energy = value; }
        public Cell Cell { get => cell; set => cell = value; }
        public Vector2Int Position { get => cell.Position; }
        public BaseAction NextAction { get => nextAction; set => nextAction = value; }
        public int MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
        public List<Spell> Spells { get => spells; set => spells = value; }
        public Sprite CorpseSprite { get => corpseSprite; private set => corpseSprite = value; }
        public Body Body { get => body; }
        public Inventory Inventory { get => inventory; }
        public SpriteRenderer SpriteRenderer { get => spriteRenderer; }
        public int RegenRate { get => regenRate; set => regenRate = value; }

        #endregion

        // Events
        public event Action<int, int> OnHealthChangeEvent;
        public event Action<Cell> OnMoveEvent;

        // Event invokers
        public void RaiseOnMoveEvent(Cell cell) => OnMoveEvent?.Invoke(cell);

        // Arbitrarily move an actor to a cell
        public static void MoveTo(Actor actor, Cell cell)
        {
            Cell previous = null;
            if (actor.cell != null)
                previous = actor.cell;

            if (!cell.IsWalkableTerrain())
            {
                UnityEngine.Debug.LogException(new Exception
                    ("MoveTo destination is not walkable"));
                return;
            }

            if (cell.Actor != null)
            {
                UnityEngine.Debug.LogException(
                    new Exception("MoveTo destination has an actor in it"));
                return;
            }

            actor.RaiseOnMoveEvent(cell);
            actor.transform.position = Helpers.V2IToV3(cell.Position);
            cell.Actor = actor;
            actor.Cell = cell;

            // Empty previous cell if one exists
            if (previous != null)
                previous.Actor = null;

            if (actor is Player)
            {
                if (cell.Items.Count > 0)
                    GameLog.LogCellItems(cell);

                if (cell.Connection != null)
                    GameLog.LogCellConnection(cell);
                else if (cell.Feature != null)
                    GameLog.LogCellFeature(cell);
            }
        }

        // Awake is called when the script instance is being loaded
        protected virtual void Awake()
        {
            if (maxHealth < 0)
                throw new Exception("Actor has negative health.");
            if (speed < 0)
                throw new Exception("Actor has negative speed.");

            health = MaxHealth;
            energy = speed;

            if (traits == null)
                traits = new List<Trait>();
            // Some actors start with traits, so these need to fire their
            // callback now
            if (traits.Count > 0)
                foreach (Trait trait in traits)
                    trait.OnGetTrait?.Invoke(this);

            body.Initialize();
        }

        protected virtual void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            Game.instance.OnTurnChangeEvent += RegenHealth;
            Game.instance.OnTurnChangeEvent += TickStatuses;
        }

        public void BuildActor(Species species)
        {
            if (body.Parts.Count > 0)
                throw new Exception("This actor is not empty.");

            this.species = species;

            foreach (BodyPartData partData in species.Parts)
                body.Parts.Add(new BodyPart(partData));
        }

        public void AssignOccupation(Occupation occ)
        {
            if (inventory.All.Count > 0)
                throw new Exception("This actor is not empty.");

            WeaponType weaponType = occ.Weapons.Random(true);
            Item weapon = ItemFactory.NewWeapon(weaponType);
            inventory.AddItem(weapon);
            new WieldAction(this, weapon, body.GetPrehensiles().ToArray()).DoAction();
        }

        // Called by scheduler to carry out and process this actor's action
        public virtual int Act()
        {
            UnityEngine.Debug.LogWarning("Attempted call of base Act()");
            return -1;
        }

        // Take a damaging hit from something
        public virtual void TakeHit(Hit hit) => TakeDamage(hit.Damage);

        public virtual void TakeDamage(int damage)
        {
            // TODO: Infinitely negative lower bound?
            health = Mathf.Clamp(health - damage, -255, MaxHealth);
            if (health <= 0)
                OnDeath();
            OnHealthChangeEvent?.Invoke(health, MaxHealth);
        }

        public void Heal(int healing)
        {
            health = Mathf.Clamp(health + healing, -255, MaxHealth);
            OnHealthChangeEvent?.Invoke(health, MaxHealth);
        }

        public void RegenHealth()
        {
            regenProgress += Game.TurnTime;

            if (regenProgress >= regenRate)
            {
                Heal(regenProgress / regenRate);
                regenProgress %= regenRate;
            }
        }

        public virtual void AddItem(Item item)
        {
            item.Owner = this;
            inventory.AddItem(item);
        }

        public virtual void RemoveItem(Item item)
        {
            item.Owner = null;
            inventory.RemoveItem(item);
        }

        public virtual void ApplyStatus(StatusEffect status)
        {
            foreach (StatusEffect s in statuses)
                if (s.Type == status.Type)
                    return;

            statuses.Add(status);
            string onApplyMsg = status.OnApply?.Invoke(this);
            GameLog.Send(onApplyMsg);
        }

        protected virtual void TickStatuses()
        {
            for (int i = statuses.Count - 1; i >= 0; i--)
            {
                statuses[i].TurnsRemaining--;
                if (statuses[i].TurnsRemaining == 0)
                {
                    string onExpireMsg = statuses[i].OnExpire?.Invoke(this);
                    statuses.RemoveAt(i);
                    GameLog.Send(onExpireMsg);
                }
            }
        }

        public virtual void AddTrait(Trait trait)
        {
            if (traits.Contains(trait))
                throw new Exception($"Actor already has trait {trait}.");

            traits.Add(trait);
            trait.OnGetTrait?.Invoke(this);
        }

        public virtual void RemoveTrait(Trait trait)
        {
            if (!traits.Contains(trait))
                throw new Exception($"Actor does not have trait {trait}.");
            else
            {
                traits.Remove(trait);
                trait.OnLoseTrait?.Invoke(this);
            }
        }

        public virtual void Convert(Faction religion)
        {
            if (religion.Type != FactionType.Religion)
                throw new ArgumentException
                    ("Faction argument must be a religion.");

            if (Faction == religion)
                throw new ArgumentException("Actor already has this faction.");

            Faction = religion;
        }

        public bool IsDead() => health < 0;

        /// <summary>
        /// Is this actor hostile to another given actor?
        /// </summary>
        /// <param name="other"></param>
        /// <returns>True if this is hostile to other.</returns>
        public bool IsHostileTo(Actor other)
        {
            if (this is Player)
            {
                NPC otherNPC = (NPC)other;

                if (otherNPC.Master == this)
                    return false;
                if (otherNPC.Faction == Faction)
                    return false;
                if (otherNPC.Faction.HostileToPlayer)
                    return true;
                if (otherNPC.AlwaysHostileToPlayer)
                    return true;

                return false;
            }
            else if (this is NPC thisNPC)
            {
                if (other is Player otherPlayer)
                {
                    if (thisNPC.Faction == otherPlayer.Faction)
                        return false;
                    if (thisNPC.AlwaysHostileToPlayer)
                        return true;
                    if (thisNPC.Faction.HostileToPlayer)
                        return true;

                    return false;
                }
                else
                    return false;

                // TODO:
                // if other's faction is hostile to this faction
                // return true
            }
            else throw new Exception("An actor illegally has the type Actor.");
        }

        // Handle this actor's death
        public virtual void OnDeath()
        {
            Game.instance.RemoveActor(this);
            cell.Actor = null;
            Destroy(gameObject);
            GameLog.Send($"You kill {Strings.GetSubject(this, false)}!",
                Strings.TextColour.White);
            cell.Items.Add(new Item(this));
        }

        // ToString override
        public override string ToString() => ActorName;
    }
}
