// SampleJSON.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon;
using Pantheon.Commands.NonActor;
using Pantheon.Components;
using Pantheon.Content;
using Pantheon.Core;
using Pantheon.Gen;
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
        [MenuItem("Assets/Pantheon/Sample JSON/Template")]
        private static void GenerateSampleTemplate()
        {
            GameObject mockGo = new GameObject();
            TurnScheduler scheduler = mockGo.AddComponent<TurnScheduler>();
            Locator.Scheduler = scheduler;

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
                        OnLandCommand = new ExplodeCommand(null)
                        {
                            Prefab = new GameObject("FX_Shrapnel"),
                            Pattern = ExplosionPattern.Square,
                            Damages = new Damage[]
                            {
                                new Damage()
                                {
                                    Type = DamageType.Searing,
                                    Min = 3,
                                    Max = 7
                                }
                            }
                        },
                    },
                new Evocable(),
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
                new Weight(50),
                new Wield(2)
            };

            EntityTemplate template = new EntityTemplate(
                "SAMPLE_ID", "SAMPLE_NAME", null, components);

            string path = Application.dataPath + $"/sample_template.json";

            if (File.Exists(path))
                File.Delete(path);

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = Binders.entity,
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

            File.AppendAllText(path, JsonConvert.SerializeObject(template, settings));

            Debug.Log($"Wrote sample template with all components to {path}.");
        }

        [MenuItem("Assets/Pantheon/Sample JSON/Builder Plan")]
        private static void GenerateSamplePlan()
        {
            BuilderPlan plan = new BuilderPlan
            {
                Steps = new BuilderStep[]
                {
                    new Fill("Terrain_StoneFloor"),
                    new RandomFill("Terrain_StoneWall", 40)
                }
            };

            string path = Application.dataPath + $"/sample_plan.json";

            if (File.Exists(path))
                File.Delete(path);

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = Binders.entity,
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter>()
                {
                    new TerrainConverter()
                }
            };

            File.AppendAllText(path, JsonConvert.SerializeObject(plan, settings));

            Debug.Log($"Wrote sample plan with all possible steps to {path}.");
        }
    }
}
