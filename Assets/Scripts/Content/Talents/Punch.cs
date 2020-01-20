// Punch.cs
// Jerome Martina

using Pantheon.Commands;
using Pantheon.Components;
using Pantheon.Utils;
using UnityEngine;

namespace Pantheon.Content.Talents
{
    public sealed class Punch : Talent
    {
        public override TargetingScheme TargetingScheme => TargetingScheme.Adjacent;
        public override Sprite Icon => Assets.Sprites["Sprite_UI_Punch"];

        public override CommandResult Cast(Entity caster)
        {
            Vector2Int target;
            if (!caster.PlayerControlled)
                target = caster.GetComponent<AI>().Target.Position;
            else
                target = Locator.Player.GetTargetedAdjacent();

            if (!caster.Level.AdjacentTo(caster.Position, target))
                throw new System.Exception(
                    $"Punch targeted non-adjacent cell.");

            Entity enemy = caster.Level.ActorAt(target);

            if (enemy == null)
            {
                AudioSource.PlayClipAtPoint(
                Assets.Audio["SFX_Toss"], target.ToVector3());
                Locator.Log.Send(
                    $"{caster.ToSubjectString(true)}" +
                    $" {Verbs.Punch(caster)} at nothing.",
                    Color.grey);
                return CommandResult.Succeeded;
            }

            AudioSource.PlayClipAtPoint(
                Assets.Audio["SFX_Punch"], target.ToVector3());

            Hit hit = new Hit(new Damage[]
                {
                    new Damage
                    {
                        Type = DamageType.Bludgeoning,
                        Min = 2,
                        Max = 4
                    }
                });
            Locator.Log.Send(
                    $"{caster.ToSubjectString(true)} " +
                    $"{Verbs.Punch(caster)} " +
                    $"{enemy.ToSubjectString(false)} " +
                    $"for {hit.TotalDamage()} damage!",
                    Color.grey);
            enemy.TakeHit(caster, hit);

            return CommandResult.Succeeded;
        }
    }
}
