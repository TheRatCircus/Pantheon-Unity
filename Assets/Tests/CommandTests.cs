// CommandTests.cs
// Jerome Martina

using NUnit.Framework;
using Pantheon;
using Pantheon.Commands;
using Pantheon.Commands.Actor;
using Pantheon.Components;
using Pantheon.World;
using UnityEngine;

namespace PantheonTests
{
    internal sealed class CommandTests
    {
        [Test]
        public void DropCommandTest()
        {
            Cell cell = new Cell(new Vector2Int(0, 0));
            Inventory inv = new Inventory(1);
            Entity actor = new Entity(inv)
            {
                Name = "TEST_ENTITY"
            };
            actor.Move(null, cell);
            Entity item = new Entity("TEST_ITEM");
            item.Move(null, cell);
            actor.GetComponent<Inventory>().AddItem(item);
            DropCommand cmd = new DropCommand(actor);
            CommandResult result = cmd.Execute(out int cost);

            Assert.True(cell.Items.Contains(item));
            Assert.True(result == CommandResult.Succeeded);
        }
    }
}
