// Relic.cs
// Jerome Martina

using Pantheon.Commands;
using Pantheon.Components;
using Pantheon.Core;
using Pantheon.Utils;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

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
            NameRelic(item);
        }

        public static void MakeMagicWeapon(Entity relic)
        {
            MagicWeaponFunctions.Random().Invoke(relic);
        }

        private static readonly Action<Entity>[] MagicWeaponFunctions
            = new Action<Entity>[]
        {
            ExplosiveMagicWeapon
        };

        private static void ExplosiveMagicWeapon(Entity relic)
        {
            GameObject fxPrefab = LoaderLocator.Service.Load<GameObject>(
                Tables.explosionFXIDs.Random());
            NonActorCommand nac;

            int r = Random.Range(0, 3);
            switch (r)
            {
                case 0:
                case 1:
                    //nac = new PointEffectCommand(null, expl);
                    //break;
                case 2:
                default:
                    ExplodeCommand expl = new ExplodeCommand(null, fxPrefab,
                        ExplosionPattern.Line,
                        new Damage()
                        {
                            Min = 7,
                            Max = 14,
                            Type = DamageType.Piercing
                        });
                    nac = new LineEffectCommand(null, expl);
                    break;
            }

            OnUse onUse = new OnUse(TurnScheduler.TurnTime, nac);
            relic.AddComponent(onUse);
        }

        private static void NameRelic(Entity relic)
        {
            TextAsset nameAsset = LoaderLocator.Service.Load<TextAsset>(
                "RelicNames");
            string[] tokens = nameAsset.text.Split(new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);
            
            int r = Random.Range(0, 10);
            switch (r)
            {
                default:
                    string noun = tokens.Random().Split(',')[0];
                    string adj = tokens.Random().Split(',')[1];
                    relic.Name = $"{adj} {noun}";
                    break;
            }
        }
    }
}
