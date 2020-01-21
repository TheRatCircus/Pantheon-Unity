// TossCommand.cs
// Jerome Martina

using Pantheon.Components;
using Pantheon.Core;
using Pantheon.Utils;
using Pantheon.World;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Pantheon.Commands.Actor
{
    public sealed class TossCommand : ActorCommand
    {
        private Entity item;

        public TossCommand(Entity entity, Entity item)
            : base(entity) => this.item = item;

        public override CommandResult Execute(out int cost)
        {
            if (Entity.PlayerControlled)
            {
                switch (Locator.Player.RequestLine(out List<Cell> line, 7))
                {
                    case InputMode.Cancelling:
                        cost = -1;
                        return CommandResult.Cancelled;
                    case InputMode.Line:
                        cost = -1;
                        return CommandResult.InProgress;
                    case InputMode.Default:
                        {
                            if (item.InInventory)
                                Entity.GetComponent<Inventory>().RemoveItem(item);

                            // Line has come through
                            GameObject tossFXObj = Object.Instantiate(
                                PrefabProvider.TossFXPrefab,
                                Entity.Cell.Position.ToVector3(),
                                new Quaternion());
                            LineProjectile proj = tossFXObj.GetComponent<LineProjectile>();
                            proj.InitializeToss(Entity, item, line);
                            proj.Fire();
                            cost = TurnScheduler.TurnTime;
                            return CommandResult.Succeeded;
                        }
                    default:
                        throw new Exception("Illegal InputMode returned.");
                }
            }
            else
                throw new NotImplementedException();
        }
    }
}
