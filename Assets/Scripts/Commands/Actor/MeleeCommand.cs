// MeleeCommand.cs
// Jerome Martina

using Pantheon.Components.Entity;
using Pantheon.Content;
using Pantheon.Utils;
using Pantheon.World;
using UnityEngine;

namespace Pantheon.Commands.Actor
{
    public sealed class MeleeCommand : ActorCommand
    {
        private readonly Cell target;

        public MeleeCommand(Entity entity, Cell target)
            : base(entity)
        {
            this.target = target;

            SpeciesDefinition species = Entity.GetComponent<Species>().SpeciesDef;

            foreach (BodyPart part in species.Parts)
            {
                if (part.Melee == null)
                    continue;

                if (part.Melee.Attacks == null)
                    continue;

                foreach (Attack atk in part.Melee.Attacks)
                {
                    if (atk.Time > Cost)
                        Cost = atk.Time;
                }
            }
        }

        public override CommandResult Execute()
        {
            Entity defender;

            if (target.Actor != null)
                defender = target.Actor;
            else
                defender = null;

            SpeciesDefinition species = Entity.GetComponent<Species>().SpeciesDef;

            foreach (BodyPart part in species.Parts)
            {
                if (part.Melee == null)
                    continue;

                if (part.Melee.Attacks == null)
                    continue;

                foreach (Attack atk in part.Melee.Attacks)
                {
                    if (atk.Accuracy < Random.Range(0, 101))
                    {
                        Locator.Log.Send(
                            Verbs.Miss(Entity, defender), Color.grey);
                        continue;
                    }

                    if (defender != null)
                    {
                        Hit hit = new Hit(atk.Damages);
                        Locator.Log.Send(
                            Verbs.Hit(Entity, defender, hit), Color.white);
                        defender.TakeHit(Entity, hit);
                    }
                }
            }

            return CommandResult.Succeeded;
        }
    }
}
