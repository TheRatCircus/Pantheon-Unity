// MultiRandomTossTalent.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon.Commands;
using Pantheon.Components.Entity;
using Pantheon.Content;
using Pantheon.Serialization.Json.Converters;
using Pantheon.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pantheon
{
    public class MultiRandomTossTalent : TossTalent
    {
        public int ProjCount { get; set; }

        private Dictionary<Entity, List<Vector2Int>> casts
            = new Dictionary<Entity, List<Vector2Int>>();

        public override HashSet<Vector2Int> GetTargetedCells(
            Entity caster, Vector2Int target)
        {
            HashSet<Vector2Int> ret = new HashSet<Vector2Int>();
            List<Vector2Int> cells = caster.Level.AdjacentCells(target);
            cells.Shuffle();
            List<Vector2Int> targets = new List<Vector2Int>();
            for (int i = 0; i < ProjCount; i++)
            {
                Line line = Bresenhams.GetLine(caster.Level, caster.Cell, cells[i]);
                targets.Add(cells[i]);
                ret.AddMany(line);
                if (ProjectileLandEffects != null)
                    foreach (ICellTalentEffect cte in ProjectileLandEffects)
                        if (cte != null)
                            ret.AddMany(cte.GetAffectedCells(caster, caster.Level, line.Last()));
            }
            if (!casts.ContainsKey(caster))
                casts.Add(caster, targets);
            return ret;
        }

        public override CommandResult Invoke(Entity caster, Vector2Int target)
        {
            if (!casts.TryGetValue(caster, out List<Vector2Int> targets))
                throw new Exception(
                    $"{caster} tried to cast without first targeting.");

            if (!caster.TryGetComponent(out Wield wield))
                throw new Exception(
                    $"{caster} tried to toss but cannot wield.");

            for (int i = 0; i < ProjCount; i++)
            {
                Line line = Bresenhams.GetLine(caster.Level, caster.Cell, targets[i]);
                Global.Instance.StartCoroutine(Fire(caster, wield.Items[0], line));
            }
            casts.Remove(caster);
            return CommandResult.Succeeded;
        }
    }
}
