// Talent.cs
// Jerome Martina

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Pantheon.Commands;
using Pantheon.Commands.NonActor;
using Pantheon.Components.Entity;
using Pantheon.Core;
using Pantheon.World;
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
        public int Range { get; set; }
        public int Time { get; set; } = TurnScheduler.TurnTime;
        public TalentTargeting Targeting { get; set; } = TalentTargeting.None;
        public TalentBehaviour Behaviour { get; set; }
        public NonActorCommand[] OnCast { get; set; }

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

        public CommandResult Cast(Entity caster, Cell target)
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

            return Behaviour.Invoke(caster, target);
        }
    }
}
