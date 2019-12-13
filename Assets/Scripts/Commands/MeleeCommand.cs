// MeleeCommand.cs
// Jerome Martina

using Pantheon.Components;
using Pantheon.UI;
using Pantheon.Util;
using Pantheon.World;
using UnityEngine;

namespace Pantheon.Commands
{
    public sealed class MeleeCommand : ActorCommand
    {
        private Cell target;
        
        public MeleeCommand(Entity entity, int attackTime, Cell target)
            : base(entity, attackTime)
        {
            this.target = target;
        }

        public override int Execute()
        {
            Entity defender;

            if (target.Actor != null)
                defender = target.Actor;
            else
                defender = null;

            SpeciesDefinition species = Entity.GetComponent<Species>().SpeciesDef;

            int attackTime = 0;

            foreach (BodyPart part in species.Parts)
            {
                if (part.Melee == null)
                    continue;

                if (part.Melee.Attacks == null)
                    continue;

                foreach (Attack atk in part.Melee.Attacks)
                {
                    if (atk.Time > attackTime)
                        attackTime = atk.Time;

                    if (atk.Accuracy < Random.Range(0, 101))
                    {
                        LogLocator._log.Send(
                            Strings.Miss(Entity, defender), Color.grey);
                        continue;
                    }
                    
                    if (defender != null)
                    {
                        Hit hit = new Hit(atk.Damages);
                        LogLocator._log.Send(
                            Strings.Hit(Entity, defender, hit), Color.white);
                        defender.TakeHit(Entity, hit);
                    }
                }
            }

            return attackTime;
        }
    }
}
