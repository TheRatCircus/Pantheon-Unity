// MeleeCommand.cs
// Jerome Martina

using Pantheon.Components;
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
            Entity defender = target.Actor != null ? target.Actor : null;

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
                        // TODO: Log miss
                        continue;
                    }
                    
                    if (defender != null)
                    {
                        // TODO: Log hit
                        Hit hit = new Hit(atk.Damages);
                        defender.TakeHit(Entity, hit);
                    }
                }
            }

            return attackTime;
        }
    }
}
