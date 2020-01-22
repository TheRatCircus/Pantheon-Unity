// Talent.cs
// Jerome Martina

using Pantheon.Commands;
using Pantheon.Utils;
using Pantheon.World;
using UnityEngine;

namespace Pantheon.Components.Talent
{
    using Entity = Pantheon.Entity;

    [System.Serializable]
    public sealed class TalentMelee : TalentComponent
    {
        public Damage[] Damages { get; set; }

        public override CommandResult Invoke(Entity caster, Cell target)
        {
            if (!caster.PlayerControlled)
                throw new System.NotImplementedException(
                    "An NPC should not be able to cast this talent.");

            if (target == null)
                throw new System.NotImplementedException(
                    "Melee talents need a target cell.");

            Entity enemy = target.Actor;

            if (enemy == null)
            {
                AudioSource.PlayClipAtPoint(
                    Assets.Audio["SFX_Toss"], target.Position.ToVector3());
                Locator.Log.Send(
                    $"{caster.ToSubjectString(true)}" +
                    $" {Verbs.Swing(caster)} at nothing.",
                    Color.grey);
                return CommandResult.Succeeded;
            }

            AudioSource.PlayClipAtPoint(
                Assets.Audio["SFX_Punch"], target.Position.ToVector3());

            Hit hit = new Hit(Damages);
            Locator.Log.Send(Verbs.Hit(caster, enemy, hit), Color.white);
            enemy.TakeHit(caster, hit);
            return CommandResult.Succeeded;
        }
    }
}