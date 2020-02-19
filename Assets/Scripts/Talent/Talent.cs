// Talent.cs
// Jerome Martina

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Pantheon.Commands;
using Pantheon.Commands.NonActor;
using Pantheon.Components.Entity;
using Pantheon.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TalentTargeting : byte
    {
        None,
        Adjacent,
        Line
    }

    [System.Serializable]
    public sealed class Talent
    {
        public string ID { get; set; } = "DEFAULT_TALENT_ID";
        public string Name { get; set; } = "DEFAULT_TALENT_NAME";
        public Sprite Icon { get; set; }
        public int PlayerTime { get; set; } = TurnScheduler.TurnTime;
        public int NPCTime { get; set; } = TurnScheduler.TurnTime;
        [JsonIgnore] public int Range => Behaviour.Range;
        public TalentBehaviour Behaviour { get; set; }
        public NonActorCommand[] OnCast { get; set; }

        [JsonIgnore] public Entity Entity { get; set; }

        public static Talent[] GetAllTalents(Entity entity)
        {
            List<Talent> ret = new List<Talent>();
            if (entity.TryGetComponent(out Talented talented))
            {
                foreach (Talent talent in talented.Talents)
                    ret.Add(talent);
            }

            if (entity.TryGetComponent(out Wield wield))
            {
                foreach (Entity item in wield.Items)
                {
                    if (item != null && item.TryGetComponent(out Evocable evoc))
                    {
                        foreach (Talent talent in evoc.Talents)
                            ret.Add(talent);
                    }
                }
            }

            return ret.ToArray();
        }

        public CommandResult Cast(Entity caster, Vector2Int target)
        {
            if (OnCast != null)
            {
                foreach (NonActorCommand nac in OnCast)
                {
                    if (nac == null)
                        continue;

                    nac.Entity = caster;
                    nac.Execute();
                }
            }

            return Behaviour.Invoke(caster, Entity, target);
        }

        public HashSet<Vector2Int> GetTargetedCells(Entity caster, Vector2Int target)
        {
            return Behaviour.GetTargetedCells(caster, Entity, target);
        }
    }
}
