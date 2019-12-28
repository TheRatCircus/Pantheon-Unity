// SampleJSON.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon;
using Pantheon.Commands.NonActor;
using Pantheon.Components;
using Pantheon.Core;
using Pantheon.Serialization.Json;
using Pantheon.Serialization.Json.Converters;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace PantheonEditor
{
    public static class SampleJSON
    {
        [MenuItem("Assets/Pantheon/Sample JSON Components")]
        private static void GenerateSampleComponents()
        {
            GameObject mockGo = new GameObject();
            TurnScheduler scheduler = mockGo.AddComponent<TurnScheduler>();
            SchedulerLocator.Service = scheduler;

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All,
                SerializationBinder = Binders._entityBinder,
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter>()
                {
                    new SpriteConverter(),
                    new TileConverter(),
                    new RuleTileConverter(),
                    new SpeciesDefinitionConverter(),
                    new BodyPartConverter()
                }
            };

            EntityComponent[] components = new EntityComponent[]
            {
                new Actor(),
                new AI(new DefaultStrategy()),
                new Ammo()
                    {
                        Type = AmmoType.Cartridges,
                        Damages = new Damage[]
                            {
                                new Damage()
                                {
                                    Type = DamageType.Piercing,
                                    Min = 3,
                                    Max = 7
                                }
                            },
                        OnLandCommand = new ExplodeCommand(
                            null, new GameObject("FX_Shrapnel"),
                            ExplosionPattern.Square,
                            new Damage[]
                            {
                                new Damage()
                                {
                                    Type = DamageType.Searing,
                                    Min = 3,
                                    Max = 7
                                }
                            })
                    },
                new Health(),
                new Inventory(20),
                new Location(),
                new Melee(
                    new MeleeAttack(
                        new Damage[]
                        {
                            new Damage()
                            {
                                Type = DamageType.Slashing,
                                Min = 2,
                                Max = 5
                            }
                        }, 80, 120)),
                new OnDamageTaken(),
                new OnUse(TurnScheduler.TurnTime),
                new Size(1),
                new Species(null),
                new Weight(50)
            };

            string path = Application.dataPath + $"/sample_components.json";

            if (File.Exists(path))
                File.Delete(path);

            foreach (EntityComponent ec in components)
            {
                File.AppendAllText(path, JsonConvert.SerializeObject(ec, settings));
                File.AppendAllText(path, System.Environment.NewLine);
            }
            Debug.Log($"Wrote samples of all components to {path}.");
        }
    }
}
