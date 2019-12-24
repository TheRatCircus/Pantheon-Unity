// Relic.cs
// Jerome Martina

using Pantheon.Core;
using Pantheon.Utils;
using System;
using UnityEngine;
using Random = UnityEngine.Random;
using Pantheon.Commands;
using Pantheon.Components;

namespace Pantheon.Gen
{
    /// <summary>
    /// Relic generation functions.
    /// </summary>
    public static class Relic
    {
        public static void MakeRelic(Entity item)
        {
            RelicArchetype archetype = RandomUtils.EnumRandom<RelicArchetype>();
            switch (archetype)
            {
                case RelicArchetype.MeleeWeapon:
                case RelicArchetype.RangedWeapon:
                case RelicArchetype.Wearable:
                case RelicArchetype.Utility:
                case RelicArchetype.MagicWeapon:
                    MakeMagicWeapon(item);
                    break;
            }
        }

        public static void MakeMagicWeapon(Entity item)
        {
            MagicWeaponFunctions.Random().Invoke(item);
        }

        private static readonly Action<Entity>[] MagicWeaponFunctions
            = new Action<Entity>[]
        {
            ExplosiveMagicWeapon
        };

        private static void ExplosiveMagicWeapon(Entity item)
        {
            GameObject fxPrefab = LoaderLocator.Service.Load<GameObject>(
                Tables.explosionFXIDs.Random());
            ExplodeCommand expl;
            NonActorCommand nac;
            
            int r = Random.Range(0, 3);
            switch (r)
            {
                case 0: // Path effect
                case 1: // Line effect
                case 2: // Point effect
                default:
                    expl = new ExplodeCommand(
                        null, fxPrefab,
                        ExplosionPattern.Point,
                        new Damage()
                        {
                            Min = 7,
                            Max = 14,
                            Type = DamageType.Piercing
                        });
                    nac = new PointEffectCommand(null, expl);
                    break;
            }

            OnUse onUse = new OnUse(TurnScheduler.TurnTime, nac);
            item.AddComponent(onUse);
        }
    }
}
