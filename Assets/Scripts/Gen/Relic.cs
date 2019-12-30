// Relic.cs
// Jerome Martina

using Pantheon.Commands.NonActor;
using Pantheon.Components;
using Pantheon.Content;
using Pantheon.Utils;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Pantheon.Gen
{
    /// <summary> Relic generation functions. </summary>
    public static class Relic
    {
        public static Entity MakeRelic()
        {
            Entity item;
            RelicArchetype archetype = RandomUtils.EnumRandom<RelicArchetype>();
            switch (archetype)
            {
                default:
                case RelicArchetype.MeleeWeapon:
                case RelicArchetype.TossingWeapon:
                    item = TossingWeaponFunctions.Random().Invoke();
                    break;
                case RelicArchetype.RangedWeapon:
                case RelicArchetype.Wearable:
                case RelicArchetype.Utility:
                case RelicArchetype.MagicWeapon:
                    item = MagicWeaponFunctions.Random().Invoke();
                    break;
            }
            Components.Relic comp = new Components.Relic();
            item.AddComponent(comp);
            NameRelic(item, comp);
            return item;
        }

        private static readonly Func<Entity>[] TossingWeaponFunctions
            = new Func<Entity>[]
        {
            MultitossWeapon
        };

        private static readonly Func<Entity>[] MagicWeaponFunctions
            = new Func<Entity>[]
        {
            ExplosiveMagicWeapon
        };

        private static Entity MultitossWeapon()
        {
            EntityTemplate basic = Locator.Loader.LoadTemplate(
                Tables.basicTossingWeapons.Random());
            Entity relic = new Entity(basic);


            GameObject fxPrefab = Locator.Loader.Load<GameObject>(
                "FX_Toss");
            NonActorCommand nac = new MultitossCommand(null, relic, 3);
            Talent talent = new Talent(11, nac);

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

            return relic;
        }

        private static Entity ExplosiveMagicWeapon()
        {
            GameObject fxPrefab = Locator.Loader.Load<GameObject>(
                Tables.explosionFXIDs.Random());
            AudioClip sfx = Locator.Loader.Load<AudioClip>("SFX_Explosion");

            NonActorCommand nac;
            ExplodeCommand expl = new ExplodeCommand(null)
            {
                Prefab = fxPrefab,
                Sound = sfx,
                Damages = new Damage[] {
                    new Damage()
                    {
                        Min = 7,
                        Max = 14,
                        Type = DamageType.Piercing
                    }
                }
            };

            int r = Random.Range(0, 2);
            switch (r)
            {
                default:
                case 0:
                    expl.Pattern = ExplosionPattern.Point;
                    nac = new PointEffectCommand(null, expl);
                    break;
                case 1:
                    expl.Pattern = ExplosionPattern.Line;
                    nac = new LineEffectCommand(null, expl);
                    break;
            }

            Talent talent = new Talent(5, nac);

            EntityTemplate basic = Locator.Loader.LoadTemplate(
                Tables.basicMagicWeapons.Random());
            Entity relic = new Entity(basic);

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

            return relic;
        }

        private static void NameRelic(Entity relic, Components.Relic comp)
        {
            TextAsset nameAsset = Locator.Loader.Load<TextAsset>(
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
                        comp.Name = $"{noun1} {noun2}";
                        break;
                    }
                case 1: // Adjective Noun
                    {
                        string noun = tokens.Random().Split(',')[0];
                        string adj = tokens.Random().Split(',')[1];
                        comp.Name = $"{adj} {noun}";
                        break;
                    }
                case 2: // Name's Noun
                    {
                        string noun = tokens.Random().Split(',')[0];
                        Markov markov = new Markov(3);
                        comp.Name = $"{markov.GetName()}'s {noun}";
                        break;
                    }
                case 3: // Adjective Number
                    {
                        string adj = tokens.Random().Split(',')[1];
                        comp.Name = $"{adj} {Random.Range(0, 1000)}";
                        break;
                    }
                case 4: // Noun Number
                    {
                        string noun = tokens.Random().Split(',')[0];
                        comp.Name = $"{noun} {Random.Range(0, 1000)}";
                        break;
                    }
                case 5: // Nounmonger
                    {
                        string noun = tokens.Random().Split(',')[0];
                        comp.Name = $"{noun}monger";
                        break;
                    }
                case 6: // Nounbringer
                    {
                        string noun = tokens.Random().Split(',')[0];
                        comp.Name = $"{noun}bringer";
                        break;
                    }
                case 7: // Noun's Noun
                    {
                        string noun1 = tokens.Random().Split(',')[0];
                        string noun2 = tokens.Random().Split(',')[0];
                        comp.Name = $"{noun1}'s {noun2}";
                        break;
                    }
                case 8: // Nounborn
                    {
                        string noun = tokens.Random().Split(',')[0];
                        comp.Name = $"{noun}born";
                        break;
                    }
                case 9: // Baseitem of Noun
                    {
                        string noun = tokens.Random().Split(',')[0];
                        comp.Name = $"{relic.Flyweight.EntityName} of {noun}";
                        break;
                    }
                default: // The Noun of Name
                    {
                        string noun = tokens.Random().Split(',')[0];
                        Markov markov = new Markov(3);
                        comp.Name = $"The {noun} of {markov.GetName()}";
                        break;
                    }
            }
        }
    }
}
