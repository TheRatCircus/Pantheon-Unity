// TossTalent.cs
// Jerome Martina

using Pantheon.Commands;
using Pantheon.Components.Entity;
using Pantheon.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Pantheon
{
    public class TossTalent : TalentBehaviour
    {
        // TODO: Setting for if projectile can hit things
        protected static readonly WaitForSeconds delay = new WaitForSeconds(.1f);

        public Damage[] Damages { get; set; }
        public int Accuracy { get; set; }
        public string ProjName { get; set; }
        public bool ProjSpins { get; set; }
        public bool ProjPierces { get; set; }
        public AudioClip HitSound { get; set; }

        public ICellTalentEffect[] ProjectileLandEffects { get; set; }
        public IEntityTalentEffect[] ProjectileHitEffects { get; set; }

        public override HashSet<Vector2Int> GetTargetedCells(
            Entity caster, Vector2Int target)
        {
            Line line = Bresenhams.GetLine(caster.Level, caster.Cell, target);
            HashSet<Vector2Int> ret = new HashSet<Vector2Int>();
            ret.AddMany(line);

            if (ProjectileLandEffects != null)
                foreach (ICellTalentEffect cte in ProjectileLandEffects)
                    if (cte != null)
                        ret.AddMany(cte.GetAffectedCells(caster, caster.Level, target));

            return ret;
        }

        public override CommandResult Invoke(Entity caster,
            Vector2Int target)
        {
            if (!caster.TryGetComponent(out Wield wield))
                throw new Exception(
                    $"{caster} tried to toss but cannot wield.");

            Line line = Bresenhams.GetLine(caster.Level, caster.Cell, target);
            Global.Instance.StartCoroutine(Fire(caster, wield.Items[0], line));
            return CommandResult.Succeeded;
        }

        protected IEnumerator Fire(Entity caster, Entity tossed, Line line)
        {
            Locator.Scheduler.Lock();
            GameObject tossFXObj = Object.Instantiate(
                Assets.TossFXPrefab,
                caster.Cell.ToVector3(),
                Quaternion.identity);

            Projectile proj = tossFXObj.GetComponent<Projectile>();
            proj.ProjName = ProjName;
            proj.Spins = ProjSpins;
            proj.Sender = caster;
            proj.Line = line;
            proj.Damages = Damages;
            proj.Pierces = ProjPierces;
            proj.Target = line[line.Count - 1];
            proj.GetComponent<SpriteRenderer>().sprite = tossed.Flyweight.Sprite;
            proj.OnLandEffects = ProjectileLandEffects;

            proj.Fire();
            yield return delay;
            Locator.Scheduler.Unlock();
        }
    }
}
