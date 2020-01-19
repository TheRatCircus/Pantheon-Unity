// Json.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon;
using Pantheon.Commands.NonActor;
using Pantheon.Components;
using Pantheon.Components.Statuses;
using Pantheon.Content;
using Pantheon.Core;
using Pantheon.Gen;
using Pantheon.Serialization.Json;
using Pantheon.Serialization.Json.Converters;
using Pantheon.Utils;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using BodyPart = Pantheon.Content.BodyPart;
using Relic = Pantheon.Components.Relic;

namespace PantheonEditor
{
    public static class Json
    {
        [MenuItem("Assets/Pantheon/JSON/New File")]
#pragma warning disable IDE0051 // Remove unused private members
        private static void NewFile()
        {
            string dataPath = Application.dataPath;
            dataPath = dataPath.Substring(0, dataPath.LastIndexOf("Assets"));
            string path = dataPath + AssetDatabase.GetAssetPath(Selection.activeObject);
            File.Create(path + $"/new.json").Close();
            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/Pantheon/JSON/Sample/Template")]
        private static void GenerateSampleTemplate()
        {
            GameObject mockGo = new GameObject();
            TurnScheduler scheduler = mockGo.AddComponent<TurnScheduler>();
            Locator.Scheduler = scheduler;

            EntityComponent[] components = new EntityComponent[]
            {
                new Actor(),
                new AI(),
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
                new Body(),
                new Corpse(),
                new Evocable(TurnScheduler.TurnTime,
                    new Talent(5, new PointEffectCommand(null,
                        new StatusCommand(null, null, new Momentum(), 1000, 1)))),
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
                new Relic() { Name = "Orb of Zot" },
                new Size(1),
                new Species(null),
                new Status(),
                new Weight(50),
                new Wield(2)
            };

            EntityTemplate template = new EntityTemplate(
                "SAMPLE_ID", "SAMPLE_NAME", null, components);

            string path = Application.dataPath + $"/Sample/sample_template.json";

            if (File.Exists(path))
                File.Delete(path);

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = Binders._entity,
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter>()
                {
                    new GameObjectConverter(),
                    new SpriteConverter(),
                    new TileConverter(),
                    new RuleTileConverter(),
                    new SpeciesDefinitionConverter(),
                    new BodyPartConverter(),
                    new StatusConverter()
                }
            };

            File.AppendAllText(path, JsonConvert.SerializeObject(template, settings));
            AssetDatabase.Refresh();
            Debug.Log($"Wrote sample template with all components to {path}.");
        }

        [MenuItem("Assets/Pantheon/JSON/Sample/Builder Plan")]
        private static void GenerateSamplePlan()
        {
            BuilderPlan plan = new BuilderPlan
            {
                Steps = new BuilderStep[]
                {
                    new BinarySpacePartition("Terrain_StoneWall", 10, true),
                    new CellularAutomata("Terrain_StoneWall", "Terrain_StoneFloor", 45),
                    new Fill("Terrain_StoneFloor"),
                    new RandomFill("Terrain_StoneWall", 40)
                },
                Population = new GenericRandomPick<string>[]
                {
                    new GenericRandomPick<string>(512, "Coyote"),
                    new GenericRandomPick<string>(512, "RagingGoose")
                }
            };

            string path = Application.dataPath + $"/Sample/sample_plan.json";

            if (File.Exists(path))
                File.Delete(path);

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = Binders._entity,
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter>()
                {
                    new TerrainConverter()
                }
            };

            File.AppendAllText(path, JsonConvert.SerializeObject(plan, settings));
            AssetDatabase.Refresh();
            Debug.Log($"Wrote sample builder plan to {path}.");
        }

        [MenuItem("Assets/Pantheon/JSON/Sample/Body Part")]
        private static void GenerateSampleBodyPart()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = Binders._entity,
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter>()
                {
                    new SpriteConverter(),
                    new SpeciesDefinitionConverter()
                }
            };

            BodyPart part = new BodyPart(
                "DEFAULT_BODYPART_ID", "DEFAULT_BODYPART_NAME",
                BodyPartType.Teeth, 0, new SpeciesDefinition("SPECIES_ID"), null);

            string path = Application.dataPath + $"/Sample/sample_bodypart.json";

            if (File.Exists(path))
                File.Delete(path);

            File.AppendAllText(path, JsonConvert.SerializeObject(part, settings));
            AssetDatabase.Refresh();
            Debug.Log($"Wrote sample body part to {path}.");
        }

        [MenuItem("Assets/Pantheon/JSON/Sample/Species")]
        private static void GenerateSampleSpecies()
        {
            SpeciesDefinition species = new SpeciesDefinition(
                "DEFAULT_SPECIES_ID", "DEFAULT_SPECIES_NAME",
                null, BodyPattern.Avian, new BodyPart("BODYPART_TEST_BEAK"));

            string path = Application.dataPath + $"/Sample/sample_species.json";

            if (File.Exists(path))
                File.Delete(path);

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = Binders._entity,
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter>()
                {
                    new SpriteConverter(),
                    new BodyPartConverter()
                }
            };

            File.AppendAllText(path, JsonConvert.SerializeObject(species, settings));
            AssetDatabase.Refresh();
            Debug.Log($"Wrote sample body part to {path}.");
        }
#pragma warning restore IDE0051 // Remove unused private members
    }
}
