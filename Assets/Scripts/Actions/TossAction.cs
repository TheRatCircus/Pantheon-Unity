// TossAction.cs
// Jerome Martina

using Pantheon.Actors;
using Pantheon.Core;
using UnityEngine;

namespace Pantheon.Actions
{
    public sealed class TossAction : BaseAction
    {
        private Item item;
        private GameObject tossFXPrefab;
        private LineProjAction proj;

        public TossAction(Actor actor, Item item)
            : base(actor)
        {
            this.item = item;

            tossFXPrefab = Database.TossFXPrefab;
            SpriteRenderer spriteRenderer = 
                tossFXPrefab.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = item.Sprite;

            if (item.OnToss != null)
            {
                proj = new LineProjAction(Actor, tossFXPrefab,
                    ProjBehaviour.Fly,
                    item.OnToss.GetAction(Actor));
            }
            else
            {
                proj = new LineProjAction(Actor, tossFXPrefab,
                    ProjBehaviour.Fly);
            }

            proj.SetSpins(true);
            proj.DoAction(AssignAction);
        }

        public override int DoAction()
        {
            if (!item.InfiniteToss)
            {
                Actor.RemoveItem(item);
            }
            return Game.TurnTime;
        }

        public override int DoAction(OnConfirm onConfirm)
            => throw new System.NotImplementedException();

        public override string ToString()
            => $"{Actor.ActorName} is throwing {item.DisplayName}.";
    }
}
