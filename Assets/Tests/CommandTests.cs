// CommandTests.cs
// Jerome Martina

using NUnit.Framework;
using Pantheon;
using Pantheon.Components.Statuses;
using Pantheon.Commands;
using Pantheon.Commands.NonActor;
using Pantheon.Components;
using UnityEngine;
using Pantheon.Core;

namespace PantheonTests
{
    internal sealed class CommandTests
    {
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
