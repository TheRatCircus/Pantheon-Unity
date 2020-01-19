// Talent.cs
// Jerome Martina

using Pantheon.Commands;
using Pantheon.Utils;
using UnityEngine;

namespace Pantheon.Content.Talents
{
    public enum TargetingScheme
    {
        Adjacent
    }

    public abstract class Talent
    {
        public abstract TargetingScheme TargetingScheme { get; }
        public abstract CommandResult Cast(Entity caster);
    }

    public sealed class Punch : Talent
    {
        public override TargetingScheme TargetingScheme => TargetingScheme.Adjacent;

        public override CommandResult Cast(Entity caster)
        {
            if (!caster.PlayerControlled)
                throw new System.NotImplementedException();

            Vector2Int target = Locator.Player.GetTargetedAdjacent();

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
