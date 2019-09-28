// CastAction.cs
// Jerome Martina

using Pantheon.Actors;

namespace Pantheon.Actions
{
    public sealed class CastAction : BaseAction
    {
        private readonly Spell spell;
        private readonly BaseAction onCast;

        private readonly int castTime;

        public CastAction(Actor actor, Spell spell)
            : base(actor)
        {
            this.spell = spell;
            onCast = spell.OnCast.GetAction(Actor);
            castTime = spell.CastTime;

            onCast.DoAction(AssignAction);
        }

        public override int DoAction() => castTime;

        public override int DoAction(OnConfirm onConfirm)
            => throw new System.NotImplementedException();

        public override string ToString()
            => $"{Actor.ActorName} is casting {spell.DisplayName}.";
    }
}