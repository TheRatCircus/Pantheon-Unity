// CommandTests.cs
// Jerome Martina

using NUnit.Framework;
using Pantheon;
using Pantheon.Components.Entity.Statuses;
using Pantheon.Commands;
using Pantheon.Commands.Actor;
using Pantheon.Commands.NonActor;
using Pantheon.Components.Entity;
using Pantheon.World;
using UnityEngine;
using Pantheon.Core;

namespace PantheonTests
{
    internal sealed class CommandTests
    {
        /// <summary>
        /// Test that an entity with an item successfully drops it to the cell
        /// it is in.
        /// </summary>
        [Test]
        public void DropCommandTest()
        {
            Level level = new Level(1, 1);
            Vector2Int cell = new Vector2Int(0, 0);
            Entity actor = new Entity(new Inventory(1))
            {
                Name = "TEST_ENTITY"
            };
            actor.Move(level, cell);
            Entity item = new Entity { Name = "TEST_ITEM" };
            item.Move(level, cell);
            actor.GetComponent<Inventory>().AddItem(item);
            DropCommand cmd = new DropCommand(actor);
            CommandResult result = cmd.Execute();

            Assert.True(level.ItemsAt(0, 0).Count > 0);
            Assert.True(result == CommandResult.Succeeded);
        }

        /// <summary>
        /// Test that an entity can pick up an item, changing all necessary
        /// state in the process.
        /// </summary>
        [Test]
        public void PickupCommandTest()
        {
            Level level = new Level(1, 1);
            Vector2Int cell = new Vector2Int(0, 0);
            Entity actor = new Entity(
                new Inventory(1),
                new Actor { Control = ActorControl.AI })
            {
                Name = "TEST_ENTITY"
            };
            actor.Move(level, cell);
            Entity item = new Entity { Name = "TEST_ITEM" };
            item.Move(level, cell);
            PickupCommand cmd = new PickupCommand(actor);
            CommandResult result = cmd.Execute();

            Assert.True(actor.GetComponent<Inventory>().Items.Contains(item));
            Assert.True(result == CommandResult.Succeeded);
        }

        /// <summary>
        /// Test that an entity which can be affected by statuses actually
        /// receives and stores one.
        /// </summary>
        [Test]
        public void StatusCommandTest()
        {
            GameObject mockGo = new GameObject();
            Locator.Scheduler = mockGo.AddComponent<TurnScheduler>();
            Entity actor = new Entity(new Status())
            {
                Name = "TEST_ENTITY"
            };
            StatusDefinition momentum = StatusDefinition.statuses["Status_Momentum"];
            StatusCommand cmd = new StatusCommand(null, actor, momentum, 300, 1);
            CommandResult result = cmd.Execute();

            Assert.True(Status.IsAffectedBy(actor, momentum));
            Assert.True(result == CommandResult.Succeeded);
        }
    }
}
