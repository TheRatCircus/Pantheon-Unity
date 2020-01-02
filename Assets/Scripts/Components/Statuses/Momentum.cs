// Momentum.cs
// Jerome Martina

using Pantheon.Util;
using UnityEngine;

namespace Pantheon.Components.Statuses
{
    /// <summary>
    /// Increases an actor's speed to 3 times its normal value.
    /// </summary>
    public sealed class Momentum : StatusDefinition
    {
        public override string ID => "Status_Momentum";
        public override string Name => "Momentum";
        public override Sprite Icon => Locator.Loader.Load<Sprite>("Sprite_Momentum");
        public override bool HasMagnitude => false;

        public override void Apply(Entity entity)
        {
            if (!entity.TryGetComponent(out Actor actor))
                return;

            Locator.Log.Send(
                $"{entity.ToSubjectString(true)} {Strings.Accelerate(entity)}!",
                Color.white);

            actor.SpeedModifier += 200;
        }

        public override void PerTurn(Entity entity) { }

        public override void Expire(Entity entity)
        {
            if (!entity.TryGetComponent(out Actor actor))
                return;

            Locator.Log.Send(
                $"{entity.ToSubjectString(true)} " +
                $"{Strings.Slow(entity)} down to normal speed.",
                Color.white);

            actor.SpeedModifier -= 200;
        }
    }
}
