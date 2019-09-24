// Player.cs
// Jerome Martina

using Pantheon.Actions;
using Pantheon.Core;
using Pantheon.Utils;
using Pantheon.World;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Pantheon.Utils.Strings;

namespace Pantheon.Actors
{
    public class Player : Actor
    {
        public PlayerInput Input { get; private set; }
        [SerializeField] protected int inventorySize = 40;
        [SerializeField] private int fovRadius = 15; // Not in cells
        private bool longResting = false;

        [SerializeField] [ReadOnly] private List<Cell> visibleCells
            = new List<Cell>();
        [SerializeField] [ReadOnly] private List<NPC> visibleEnemies
            = new List<NPC>();
        [SerializeField] [ReadOnly] private List<Cell> movePath;

        // Properties
        public int InventorySize { get => inventorySize; }
        public int FOVRadius { get => fovRadius; }
        public List<Cell> MovePath { get => movePath; set => movePath = value; }

        // Events
        public event Action OnInventoryChangeEvent;
        public event Action OnInventoryToggleEvent;
        public event Action<List<StatusEffect>> StatusChangeEvent;
        public event Action OnPlayerDeathEvent;

        // Event invokers for PlayerInput
        public void RaiseInventoryToggleEvent() => OnInventoryToggleEvent?.Invoke();

        public void Initialize()
        {
            Inventory = new Inventory(40);
        }

        // Awake is called when the script instance is being loaded
        protected override void Awake()
        {
            base.Awake();
            movePath = new List<Cell>();
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            
            Input = GetComponent<PlayerInput>();
            UnityEngine.Debug.Log("Player input registered.");
        }

        // Request this actor's action and carry it out
        public override int Act()
        {
            if (longResting)
            {
                if (visibleEnemies.Count > 0)
                {
                    GameLog.Send($"An enemy is nearby!", TextColour.Red);
                    longResting = false;
                    return -1;
                }

                if (health >= maxHealth)
                {
                    StopLongRest();
                    return -1;
                }

                return new WaitAction(this).DoAction();
            }

            if (movePath.Count > 0)
            {
                if (visibleEnemies.Count > 0)
                {
                    GameLog.Send($"An enemy is nearby!", TextColour.Red);
                    movePath.Clear();
                    return -1;
                }

                Cell destination = movePath[0];
                movePath.RemoveAt(0);
                return new MoveAction(this, MoveSpeed, destination).DoAction();
            }

            if (NextAction != null)
            {
                BaseAction ret = NextAction;
                // Clear action buffer
                NextAction = null;
                return ret.DoAction();
            }

            else return -1;
        }

        public override void TakeHit(Hit hit)
        {
            if (!Game.instance.IdolMode)
                TakeDamage(hit.Damage);
        }

        public void LongRest()
        {
            if (visibleEnemies.Count > 0)
            {
                GameLog.Send($"An enemy is nearby!", TextColour.Red);
                return;
            }

            string restMsg = RestMessages.Random(false);
            GameLog.Send($"You stop to {restMsg}", TextColour.Grey);
            longResting = true;
        }

        public void StopLongRest()
        {
            GameLog.Send("You finish your rest.", TextColour.Grey);
            longResting = false;
        }

        public override void ApplyStatus(StatusEffect status)
        {
            base.ApplyStatus(status);
            StatusChangeEvent?.Invoke(statuses);
        }

        protected override void TickStatuses()
        {
            base.TickStatuses();
            StatusChangeEvent?.Invoke(statuses);
        }

        public override void AddItem(Item item)
        {
            base.AddItem(item);
            OnInventoryChangeEvent?.Invoke();
        }

        public override void RemoveItem(Item item)
        {
            base.RemoveItem(item);
            OnInventoryChangeEvent?.Invoke();
        }

        public override void Convert(Faction religion)
        {
            if (religion.Type != FactionType.Religion)
                throw new ArgumentException
                    ("Faction argument must be a religion.");

            if (Faction == religion)
            {
                GameLog.Send($"You are already a member of {religion}!");
                return;
            }

            Faction = religion;
            GameLog.Send($"You are now a member of {religion}!");
        }

        // Call every time FOV is refreshed
        public void UpdateVisibleCells(List<Cell> refreshed)
        {
            visibleCells.Clear();
            visibleEnemies.Clear();

            foreach (Cell c in refreshed)
                if (c.Visible) visibleCells.Add(c);

            // Also refresh list of visible enemies
            foreach (Cell c in visibleCells)
                if (c.Actor is NPC)
                    visibleEnemies.Add((NPC)c.Actor);
        }

        public override void OnDeath()
        {
            cell.Actor = null;
            GameLog.Send("You perish...", TextColour.Purple);
            OnPlayerDeathEvent?.Invoke();
        }
    }
}
