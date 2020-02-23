// BlinkTalent.cs
// Jerome Martina

using Pantheon.Commands;
using Pantheon.Components.Entity;
using Pantheon.Utils;
using Pantheon.World;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon
{
    public class BlinkTalent : TalentBehaviour
    {
        public bool Random { get; set; }
        public bool CanDisplace { get; set; }
        public AudioClip Sound { get; set; }

        public override HashSet<Vector2Int> GetTargetedCells(
            Entity caster, Vector2Int target)
        {
            if (Random)
                return new HashSet<Vector2Int>();
            else
                return new HashSet<Vector2Int>()
                {
                    Bresenhams.GetLine(
                        caster.Level,
                        caster.Cell,
                        target).ElementAtOrLast(Range)
                };
        }

        public override CommandResult Invoke(Entity caster, Vector2Int target)
        {
            CommandResult result;

            if (Random)
                result = RandomBlink(caster);
            else
                result = ControlledBlink(caster, target);

            if (result == CommandResult.Succeeded)
                Locator.Log.Send(Verbs.Blink(caster), Color.white);

            return CommandResult.Succeeded;
        }

        private CommandResult RandomBlink(Entity caster)
        {
            // TODO: Support for NPCs
            Locator.Audio.Buffer(Sound, caster.Cell.ToVector3());
            Vector2Int cell = Level.NullCell;
            while (!caster.Level.Walkable(cell))
                cell = Locator.Player.VisibleCells.Random();
            caster.Move(caster.Level, cell);
            return CommandResult.Succeeded;
        }

        private CommandResult ControlledBlink(Entity caster, Vector2Int target)
        {
            bool player = Actor.PlayerControlled(caster);

            Vector2Int cell = Bresenhams.GetLine(
                        caster.Level,
                        caster.Cell,
                        target).ElementAtOrLast(Range);

            if (cell == Level.NullCell)
            {
                if (player)
                    Locator.Log.Send(
                        $"You have to supply a target cell.",
                        Color.grey);
                else
                    UnityEngine.Debug.LogWarning(
                        $"{caster} attempted to blink to a null cell.");
                return CommandResult.Failed;
            }

            // TODO: Support for CanDisplace
            if (!caster.Level.Walkable(cell))
            {
                if (player)
                    Locator.Log.Send(
                        $"Target a walkable cell, please.",
                        Color.grey);
                else
                    UnityEngine.Debug.LogWarning(
                        $"{caster} attempted to blink to {cell} (unwalkable).");
                return CommandResult.Failed;
            }

            Locator.Audio.Buffer(Sound, caster.Cell.ToVector3());
            caster.Move(caster.Level, cell);
            return CommandResult.Succeeded;
        }
    }
}
