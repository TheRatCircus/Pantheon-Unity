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
    }

    [System.Serializable]
    public sealed class Talent
    {
        public string Name { get; set; }
        public Sprite Icon { get; set; }
        public int Range { get; set; }
        public int Time { get; set; } = TurnScheduler.TurnTime;
        public TalentTargeting Targeting { get; set; } = TalentTargeting.None;
        public TalentComponent[] Components { get; set; }
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
            CommandResult result = CommandResult.Succeeded;

            if (OnCast != null)
            {
                foreach (NonActorCommand nac in OnCast)
                {
                    if (nac == null)
                        continue;

                    nac.Entity = caster;
                    CommandResult r = nac.Execute();

                    if (r == CommandResult.InProgress)
                        result = CommandResult.InProgress;
                    if (r == CommandResult.Cancelled)
                        result = CommandResult.Cancelled;
                }
            }

            foreach (TalentComponent component in Components)
            {
                CommandResult r = component.Invoke(caster, target);
                if (r == CommandResult.InProgress)
                    result = CommandResult.InProgress;
                if (r == CommandResult.Cancelled)
                    result = CommandResult.Cancelled;
            }

            return result;
        }
    }
}
