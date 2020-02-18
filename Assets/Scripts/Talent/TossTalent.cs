// TossTalent.cs
// Jerome Martina

using Pantheon.Commands;
using Pantheon.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.Components.Talent
{
    using Entity = Pantheon.Entity;

    public sealed class TossTalent : TalentBehaviour
    {
        private static readonly WaitForSeconds delay = new WaitForSeconds(.1f);

        public Damage[] Damages { get; set; }
        public int Accuracy { get; set; }
        public AudioClip HitSound { get; set; }

        public override HashSet<Vector2Int> GetTargetedCells(
            Entity caster, Entity evoked, Vector2Int target)
        {
            Line line = Bresenhams.GetLine(caster.Level, caster.Cell, target);
            HashSet<Vector2Int> ret = new HashSet<Vector2Int>();
            ret.AddMany(line);
            return ret;
        }

        public override CommandResult Invoke(Entity caster,
            Entity evoked, Vector2Int target)
        {
            Line line = Bresenhams.GetLine(caster.Level, caster.Cell, target);
            Global.Instance.StartCoroutine(Fire(caster, evoked, line));
            return CommandResult.Succeeded;
        }

        private IEnumerator Fire(Entity caster, Entity evoked, Line line)
        {
            Locator.Scheduler.Lock();
            GameObject tossFXObj = Object.Instantiate(
                Assets.TossFXPrefab,
                caster.Cell.ToVector3(),
                new Quaternion());
            LineProjectile proj = tossFXObj.GetComponent<LineProjectile>();
            proj.InitializeToss(caster, evoked, line);
            proj.Fire();
            yield return delay;
            Locator.Scheduler.Unlock();
        }
    }
}
