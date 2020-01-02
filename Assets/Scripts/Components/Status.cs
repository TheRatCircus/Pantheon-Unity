// Status.cs
// Jerome Martina

using Pantheon.Components.Statuses;
using Pantheon.Core;
using System.Collections.Generic;

namespace Pantheon.Components
{
    [System.Serializable]
    public sealed class Status : EntityComponent
    {
        public List<StatusEffect> Statuses { get; private set; }
            = new List<StatusEffect>();

        public Status() => Locator.Scheduler.ClockTickEvent += TickStatuses;

        public Status(List<StatusEffect> statuses) : this() => Statuses = statuses;

        private void TickStatuses()
        {
            for (int i = Statuses.Count - 1; i >= 0; i--)
            {
                if (Statuses[i].Tick(Entity))
                {
                    Statuses[i].Definition.Expire(Entity);
                    Statuses.RemoveAt(i);
                }
            }
        }

        public override EntityComponent Clone(bool full)
        {
            return new Status(new List<StatusEffect>(Statuses));
        }

        public static void ApplyStatus(Entity entity, StatusEffect effect)
        {
            if (!entity.TryGetComponent(out Status status))
                return;

            status.Statuses.Add(effect);
            effect.Definition.Apply(entity);
        }
    }

    public sealed class StatusEffect
    {
        public StatusDefinition Definition { get; private set; }
        public int Time { get; set; }
        public int Magnitude { get; set; }

        public StatusEffect(StatusDefinition def, int time, int magnitude)
        {
            Definition = def;
            Time = time;
            Magnitude = magnitude;
        }

        public bool Tick(Entity entity)
        {
            Time -= TurnScheduler.TurnTime;
            Definition.PerTurn(entity);
            if (Time <= 0)
                return true;
            else
                return false;
        }
    }
}
