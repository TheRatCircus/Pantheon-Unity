// Actor.cs
// Jerome Martina

using System;
using System.Collections.Generic;
using UnityEngine;
using Pantheon.Core;
using Pantheon.World;
using Pantheon.Utils;
using Pantheon.Actions;

namespace Pantheon.Actors
{
    /// <summary>
    /// Any entity which acts with any degree of agency.
    /// </summary>
    public class Actor : MonoBehaviour
    {
        protected List<Item> inventory;

        // Locational
        public Level level;
        [SerializeField] protected Cell cell;

        // Actor's personal attributes
        [SerializeField] protected string actorName;
        [SerializeField] protected bool nameIsProper; // TODO: Unique mobs
        [SerializeField] [ReadOnly] protected int health;
        [SerializeField] protected int maxHealth = -1;
        [SerializeField] protected int regenRate = -1; // Time to regen 1 HP
        [SerializeField] [ReadOnly] protected int regenProgress = 0;
        [SerializeField] protected int speed = -1; // Energy per turn
        [SerializeField] [ReadOnly] protected int energy; // Energy remaining
        [SerializeField] protected int moveSpeed; // Energy needed to walk one cell

        [SerializeField]
        [ReadOnly]
        protected List<StatusEffect> statuses = new List<StatusEffect>();

        // Per-actor-type data
        [SerializeField] protected CorpseType corpse;
        [SerializeField] protected List<BodyPart> parts;

        // Action status
        [SerializeField] [ReadOnly] protected BaseAction nextAction;

        #region Properties

        public string ActorName { get => actorName; set => actorName = value; }
        public bool NameIsProper { get => nameIsProper; set => nameIsProper = value; }
        public int Health { get => health; }
        public int MaxHealth { get => maxHealth; }
        public int Speed { get => speed; set => speed = value; }
        public int Energy { get => energy; set => energy = value; }
        public Cell Cell { get => cell; set => cell = value; }
        public Vector2Int Position { get => cell.Position; }
        public List<Item> Inventory { get => inventory; }
        public BaseAction NextAction { get => nextAction; set => nextAction = value; }
        public List<BodyPart> Parts { get => parts; }
        public int MoveSpeed { get => moveSpeed; }

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
                Debug.LogException(new Exception("MoveTo destination is not walkable"));
                return;
            }

            if (cell.Actor != null)
            {
                Debug.LogException(new Exception("MoveTo destination has an actor in it"));
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
                GameLog.LogCellItems(cell);
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
            
            foreach (BodyPart part in parts)
                part.Initialize();
        }

        protected virtual void Start()
        {
            Game.instance.OnTurnChangeEvent += RegenHealth;
            Game.instance.OnTurnChangeEvent += TickStatuses;
        }

        // Called by scheduler to carry out and process this actor's action
        public virtual int Act()
        { Debug.LogWarning("Attempted call of base Act()"); return -1; }

        // Take a damaging hit from something
        public void TakeHit(Hit hit) => TakeDamage(hit.Damage);

        public void TakeDamage(int damage)
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

        // Remove an item from this actor's inventory
        public virtual void RemoveItem(Item item)
        {
            inventory.Remove(item);
            item.Owner = null;
        }

        // Check if this actor has any prehensile body parts
        public bool HasPrehensile()
        {
            foreach (BodyPart part in parts)
                if (part.Prehensile) return true;

            return false;
        }

        public List<BodyPart> GetPrehensiles()
        {
            List<BodyPart> prehensiles = new List<BodyPart>();

            foreach (BodyPart part in parts)
                if (part.Prehensile)
                    prehensiles.Add(part);

            return prehensiles;
        }

        public bool IsDead() => health < 0;

        /// <summary>
        /// Get all the melee attacks this actor can possibly perform.
        /// </summary>
        /// <returns></returns>
        public List<Melee> GetMelees()
        {
            List<Melee> melees = new List<Melee>();

            foreach (BodyPart part in parts)
            {
                if (part.Item != null)
                    melees.Add(part.Item.Melee);
                else if (part.CanMelee)
                    melees.Add(part.Melee);
                else
                    continue;
            }

            if (melees.Count == 0)
                return null;
            else
                return melees;
        }

        /// <summary>
        ///  Check if the cumulative strength of the actor's prehensiles used to
        ///  wield an item meet that item's strength requirement.
        /// </summary>
        /// <param name="req">The strength requirement checked against.</param>
        /// <returns>True if the actor has enough strength over all its prehensiles.</returns>
        public bool MeetsStrengthReq(Item item)
        {
            if (item.StrengthReq == 0)
                return true;

            int wieldStrength = 0;

            List<BodyPart> prehensiles = GetPrehensiles();
            foreach (BodyPart prehensile in prehensiles)
                if (prehensile.Item == item)
                    wieldStrength += prehensile.Strength;

            if (wieldStrength >= item.StrengthReq)
                return true;
            else
                return false;
        }

        // Check if another actor is hostile to this
        public bool HostileToMe(Actor other)
        {
            if (this is Player) // Everything else is hostile to player (for now)
                return true;
            else // This is an NPC
                return other is Player; // If other is Player, it's hostile
        }

        // Handle this actor's death
        protected virtual void OnDeath()
        {
            Game.instance.RemoveActor(this);
            cell.Actor = null;
            Destroy(gameObject);
            GameLog.Send($"You kill {Strings.GetSubject(this, false)}!",
                Strings.TextColour.White);
            cell.Items.Add(new Item(Database.GetCorpse(corpse)));
        }

        // ToString override
        public override string ToString() => ActorName;
    }
}
