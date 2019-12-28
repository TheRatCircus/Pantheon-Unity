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

            Talent talent = new Talent(nac);

            if (relic.TryGetComponent(out Evocable evoc))
            {
                evoc.AddTalent(talent);
            }
            else
            {
                Evocable newEvoc = new Evocable(talent);
                newEvoc.AddTalent(talent);
                relic.AddComponent(newEvoc);
            }
        }

        private static void NameRelic(Entity relic)
        {
            TextAsset nameAsset = LoaderLocator.Service.Load<TextAsset>(
                "RelicNames");
            string[] tokens = nameAsset.text.Split(new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);
            
            int r = Random.Range(0, 11);
            switch (r)
            {
                case 0: // Noun Noun
                    {
                        string noun1 = tokens.Random().Split(',')[0];
                        string noun2 = tokens.Random().Split(',')[0];
                        relic.Name = $"{noun1} {noun2}";
                        break;
                    }
                case 1: // Adjective Noun
                    {
                        string noun = tokens.Random().Split(',')[0];
                        string adj = tokens.Random().Split(',')[1];
                        relic.Name = $"{adj} {noun}";
                        break;
                    }
                case 2: // Name's Noun
                    {
                        string noun = tokens.Random().Split(',')[0];
                        Markov markov = new Markov(3);
                        relic.Name = $"{markov.GetName()}'s {noun}";
                        break;
                    }
                case 3: // Adjective Number
                    {
                        string adj = tokens.Random().Split(',')[1];
                        relic.Name = $"{adj} {Random.Range(0, 1000)}";
                        break;
                    }
                case 4: // Noun Number
                    {
                        string noun = tokens.Random().Split(',')[0];
                        relic.Name = $"{noun} {Random.Range(0, 1000)}";
                        break;
                    }
                case 5: // Nounmonger
                    {
                        string noun = tokens.Random().Split(',')[0];
                        relic.Name = $"{noun}monger";
                        break;
                    }
                case 6: // Nounbringer
                    {
                        string noun = tokens.Random().Split(',')[0];
                        relic.Name = $"{noun}bringer";
                        break;
                    }
                case 7: // Noun's Noun
                    {
                        string noun1 = tokens.Random().Split(',')[0];
                        string noun2 = tokens.Random().Split(',')[0];
                        relic.Name = $"{noun1}'s {noun2}";
                        break;
                    }
                case 8: // Nounborn
                    {
                        string noun = tokens.Random().Split(',')[0];
                        relic.Name = $"{noun}born";
                        break;
                    }
                case 9: // Baseitem of Noun
                    {
                        string noun = tokens.Random().Split(',')[0];
                        relic.Name = $"{relic.Flyweight.EntityName} of {noun}";
                        break;
                    }
                default: // The Noun of Name
                    {
                        string noun = tokens.Random().Split(',')[0];
                        Markov markov = new Markov(3);
                        relic.Name = $"The {noun} of {markov.GetName()}";
                        break;
                    }
            }
        }
    }
}
