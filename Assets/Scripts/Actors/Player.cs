// Player.cs
// Jerome Martina

using System;
using System.Collections.Generic;
using UnityEngine;
using Pantheon.Core;
using Pantheon.World;
using Pantheon.Actions;
using Pantheon.Utils;
using static Pantheon.Utils.Strings;

namespace Pantheon.Actors
{
    public class Player : Actor
    {
        private PlayerInput input;

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
        public PlayerInput Input { get => input; }
        public List<Cell> MovePath { get => movePath; set => movePath = value; }

        // Events
        public event Action OnInventoryChangeEvent;
        public event Action OnInventoryToggleEvent;
        public event Action<List<StatusEffect>> StatusChangeEvent;
        public event Action OnPlayerDeathEvent;

        // Event invokers for PlayerInput
        public void RaiseInventoryToggleEvent() => OnInventoryToggleEvent?.Invoke();
        public void RaiseInventoryChangeEvent() => OnInventoryChangeEvent?.Invoke();

        // Awake is called when the script instance is being loaded
        protected override void Awake()
        {
            base.Awake();
            inventory = new List<Item>(inventorySize);
            movePath = new List<Cell>();
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            input = GetComponent<PlayerInput>();
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

            string restMsg = RandomUtils.ArrayRandom(Strings.RestMessages);
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

        // Remove an item from this actor's inventory
        public override void RemoveItem(Item item)
        {
            base.RemoveItem(item);
            OnInventoryChangeEvent?.Invoke();
        }

        // Update the list of cells visible to this player
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

        // Handle the player's death
        public override void OnDeath()
        {
            cell.Actor = null;
            GameLog.Send("You perish...", TextColour.Purple);
            OnPlayerDeathEvent?.Invoke();
        }
    }
}
