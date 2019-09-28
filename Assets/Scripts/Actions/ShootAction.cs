// ShootAction.cs
// Jerome Martina

using Pantheon.Actors;
using Pantheon.Core;
using Pantheon.World;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.Actions
{
    public sealed class ShootAction : BaseAction
    {
        private List<Cell> targetLine;

        public ShootAction(Actor actor, List<Cell> targetLine)
            : base(actor)
        {
            this.targetLine = targetLine;

            List<Item> rangedWeapons = new List<Item>();

            foreach (Item item in Actor.Inventory.Wielded)
                if (item.IsRanged)
                    rangedWeapons.Add(item);

            // Keep in parallel
            List<LineProjAction> projectiles = new List<LineProjAction>();
            List<Ammo> ammoUsed = new List<Ammo>();

            foreach (Item weapon in rangedWeapons)
            {
                if (Actor.Inventory.HasAmmoFor(weapon))
                {
                    Ammo ammo = null;
                    foreach (Item item in Actor.Inventory.All)
                    {
                        if (item.Ammo.AmmoFamily == weapon.Ranged.AmmoFamily)
                        {
                            ammo = item.Ammo;
                            break;
                        }
                    }

                    if (ammo == null)
                    {
                        GameLog.Send("You lack ammunition with which to fire!",
                                Utils.Strings.TextColour.Grey);
                        return;
                    }

                    GameObject shotPrefab = ammo.FXPrefab;
                    ammoUsed.Add(ammo);
                    projectiles.Add(new LineProjAction(Actor, shotPrefab, ProjBehaviour.Instant));
                }
            }

            int i = 0;
            for (; i < projectiles.Count - 1; i++)
            {
                projectiles[i].SetLine(targetLine);
                projectiles[i].SetValues(ammoUsed[i]);
                projectiles[i].DoAction();
            }

            projectiles[i].SetValues(ammoUsed[i]);
            projectiles[i].SetLine(targetLine);
            projectiles[i].DoAction(AssignAction);
        }

        public override int DoAction() => Game.TurnTime;

        public override int DoAction(OnConfirm onConfirm)
            => throw new System.NotImplementedException();

        public override string ToString()
            => $"{Actor.ActorName} is shooting.";
    }
}