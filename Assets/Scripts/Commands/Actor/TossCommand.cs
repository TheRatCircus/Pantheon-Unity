// TossCommand.cs
// Jerome Martina

using Pantheon.Components.Entity;
using Pantheon.Core;
using Pantheon.Utils;
using Pantheon.World;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Pantheon.Commands.Actor
{
    using Actor = Components.Entity.Actor;

    public sealed class TossCommand : ActorCommand
    {
        private readonly Entity item;

        public TossCommand(Entity entity, Entity item)
            : base(entity)
        {
            this.item = item;
            Cost = TurnScheduler.TurnTime;
        }

        public override CommandResult Execute()
        {
            if (Actor.PlayerControlled(Entity))
            {
                switch (Locator.Player.RequestLine(out Line line, 7))
                {
                    case InputMode.Cancelling:
                        return CommandResult.Cancelled;
                    case InputMode.Line:
                        return CommandResult.InProgress;
                    case InputMode.Default:
                        {
                            if (item.InInventory)
                                Entity.GetComponent<Inventory>().RemoveItem(item);

                            // Line has come through
                            GameObject tossFXObj = Object.Instantiate(
                                Assets.TossFXPrefab,
                                Entity.Cell.ToVector3(),
                                new Quaternion());
                            LineProjectile proj = tossFXObj.GetComponent<LineProjectile>();
                            proj.InitializeToss(Entity, item, line);
                            proj.Fire();
                            
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
