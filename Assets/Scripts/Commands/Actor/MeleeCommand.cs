// MeleeCommand.cs
// Jerome Martina

using Pantheon.Components.Entity;
using Pantheon.Content;
using Pantheon.Utils;
using UnityEngine;

namespace Pantheon.Commands.Actor
{
    public sealed class MeleeCommand : ActorCommand
    {
        private readonly Vector2Int target;

        public MeleeCommand(Entity entity, Vector2Int target) : base(entity)
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
            Entity defender = Entity.Level.ActorAt(target);

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
