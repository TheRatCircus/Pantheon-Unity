// EntityMessageTests.cs
// Jerome Martina

using NUnit.Framework;
using Pantheon;
using Pantheon.Commands.NonActor;
using Pantheon.Components.Entity;
using Pantheon.UI;

namespace PantheonTests
{
    internal sealed class EntityMessageTests
    {
        /// <summary>
        /// Test that an entity broadcasts messages between components.
        /// </summary>
        [Test]
        public void EntityReceivesMessages()
        {
            Locator.Log = new MockGameLog();

            Entity entity = new Entity();
            Actor actor = new Actor(ActorControl.AI);
            entity.AddComponent(actor);
            Health health = new Health(10, 800);
            entity.AddComponent(health);
            entity.AddComponent(new OnDamageTaken(
                new ActorControlCommand(entity, null, ActorControl.Player)));
            health.Damage(null, new HitDamage(DamageType.Slashing, 5));
            Assert.True(actor.Control == ActorControl.Player);
        }
    }
}
