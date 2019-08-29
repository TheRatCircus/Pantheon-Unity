// 
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.Actions
{
    public class LineProjAction : BaseAction
    {
        GameObject projPrefab;
        public delegate void OnConfirmDelegate();
        public OnConfirmDelegate onConfirm;

        List<Cell> line;

        public LineProjAction(Actor actor, GameObject projPrefab, OnConfirmDelegate onConfirm) : base(actor)
        {
            ActionCost = -1; // Real cost is from casting spell, using item, etc.
            this.projPrefab = projPrefab;
            this.onConfirm = onConfirm;

            if (actor is Player)
                ((Player)actor)._input.StartCoroutine(((Player)actor)._input.LineTarget(AssignAction));
            else
                throw new System.NotImplementedException("An NPC should not be able to do this");
        }

        public override void AssignAction()
        {
            line = ((Player)actor)._input.TargetLine;
            onConfirm();
        }

        public override int DoAction()
        {
            MakeEntity.MakeLineProjectile(projPrefab, line);
            return ActionCost;
        }
    }
}
