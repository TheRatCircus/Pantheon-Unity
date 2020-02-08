// TossTalent.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon.Commands;
using Pantheon.Utils;
using Pantheon.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.Components.Talent
{
    using Entity = Pantheon.Entity;

    public sealed class TossTalent : TalentBehaviour, IEntityBasedTalent
    {
        private static readonly WaitForSeconds delay = new WaitForSeconds(.1f);

        public Damage[] Damages { get; set; }
        public int Accuracy { get; set; }
        public AudioClip HitSound { get; set; }

        [JsonIgnore] public Entity Entity { get; set; }

        public override HashSet<Cell> GetTargetedCells(Entity caster, Cell target)
        {
            Line line = Bresenhams.GetLine(caster.Level, caster.Cell, target);
            HashSet<Cell> ret = new HashSet<Cell>();
            ret.AddMany(line);
            return ret;
        }

        public override CommandResult Invoke(Entity caster, Cell target)
        {
            Line line = Bresenhams.GetLine(caster.Level, caster.Cell, target);
            Global.Instance.StartCoroutine(Fire(caster, line));
            return CommandResult.Succeeded;
        }

        private IEnumerator Fire(Entity caster, Line line)
        {
            Locator.Scheduler.Lock();
            GameObject tossFXObj = Object.Instantiate(
                Assets.TossFXPrefab,
                caster.Cell.Position.ToVector3(),
                new Quaternion());
            LineProjectile proj = tossFXObj.GetComponent<LineProjectile>();
            proj.InitializeToss(caster, Entity, line);
            proj.Fire();
            yield return delay;
            Locator.Scheduler.Unlock();
        }
    }
}
