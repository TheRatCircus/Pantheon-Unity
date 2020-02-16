// Json.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon;
using Pantheon.Commands.NonActor;
using Pantheon.Components.Entity;
using Pantheon.Components.Talent;
using Pantheon.Content;
using Pantheon.Core;
using Pantheon.Gen;
using Pantheon.Serialization.Json;
using Pantheon.Serialization.Json.Converters;
using Pantheon.Utils;
using Pantheon.World;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using BodyPart = Pantheon.Content.BodyPart;
using Relic = Pantheon.Components.Entity.Relic;

namespace PantheonEditor
{
    public static class Json
    {
#pragma warning disable IDE0051
        [MenuItem("Assets/Pantheon/JSON/New File")]
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
            Assets.LoadAssets();
            GameObject mockGo = new GameObject();
            GameObject fxShrapnelPrefab = new GameObject("FX_Shrapnel");
            TurnScheduler scheduler = mockGo.AddComponent<TurnScheduler>();
            Locator.Scheduler = scheduler;

            EntityComponent[] components = new EntityComponent[]
            {
                new Actor() { Control = ActorControl.None },
                new AI(new AIDefinition
                {
                    ID = "SAMPLE_AI_ID",
                    Utilities = new AIUtility[]
                    {
                        new ApproachUtility(),
                        new AttackUtility()
                    }
                    }),
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
                            Prefab = fxShrapnelPrefab,
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
                new Dialogue(),
                new Evocable(),
                new Health(),
                new Inventory(20),
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
                new Splat() { Sound = null },
                new Status(),
                new Talented()
                {
                    Talents = new Talent[]
                    {
                        new Talent()
                        {
                            ID = "Talent_Foobar"
                        },
                    }
                },
                new Weight(50),
                new Wield(2)
            };

            EntityTemplate template = new EntityTemplate
            {
                ID = "SAMPLE_ID",
                EntityName = "SAMPLE_NAME",
                Flags = EntityFlag.Unique | EntityFlag.Blocking,
                Components = components,
                Inventory = new EntityTemplate[]
                {
                    new EntityTemplate
                    {
                        ID = "Item_SampleItem"
                    }
                },
                Wielded = new EntityTemplate[]
                {
                    new EntityTemplate
                    {
                        ID = "Item_SampleWeapon"
                    }
                }
            };

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
                    new AIDefinitionConverter(),
                    new GameObjectConverter(),
                    new SpriteConverter(),
                    new TalentConverter(),
                    new TileConverter(),
                    new RuleTileConverter(),
                    new SpeciesDefinitionConverter(),
                    new BodyPartConverter(),
                    new StatusConverter()
                }
            };

            File.AppendAllText(path, JsonConvert.SerializeObject(template, settings));
            AssetDatabase.Refresh();
            Object.DestroyImmediate(mockGo as GameObject);
            Object.DestroyImmediate(fxShrapnelPrefab as GameObject);
            Debug.Log($"Wrote sample template with all components to {path}.");
            Assets.UnloadAssets();
        }

        [MenuItem("Assets/Pantheon/JSON/Sample/Builder")]
        private static void GenerateSampleBuilder()
        {
            TerrainDefinition ground = ScriptableObject.CreateInstance<TerrainDefinition>();
            ground.name = "Terrain_StoneFloor";
            TerrainDefinition wall = ScriptableObject.CreateInstance<TerrainDefinition>();
            wall.name = "Terrain_StoneWall";
            Tile mockTile = ScriptableObject.CreateInstance<Tile>();
            mockTile.name = "StoneStairs_Up";

            Builder builder = new Builder
            {
                ID = "SAMPLE_ID",
                LevelID = "SAMPLE_ID",
                DisplayName = "SAMPLE_NAME",
                Position = new Vector3Int(-2, 1, 1),
                Ground = ground,
                Wall = wall,
                Size = new Vector2Int(80, 80),
                Steps = new BuilderStep[]
                {
                    new BinarySpacePartition("Terrain_StoneWall", 10, true),
                    new CellularAutomata("Terrain_StoneWall", "Terrain_StoneFloor", 45),
                    new Fill("Terrain_StoneFloor"),
                    new RandomFill("Terrain_StoneWall", 40)
                },
                ConnectionRules = new ConnectionRule[]
                {
                    new ConnectionRule()
                    {
                        Count = 1,
                        Key = "Reformatory",
                        Tile = mockTile
                    }
                },
                Population = new GenericRandomPick<string>[]
                {
                    new GenericRandomPick<string>(512, "Coyote"),
                    new GenericRandomPick<string>(512, "RagingGoose")
                }
            };

            string path = Application.dataPath + $"/Sample/sample_builder.json";

            if (File.Exists(path))
                File.Delete(path);

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = Binders._entity,
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter>()
                {
                    new RuleTileConverter(),
                    new TerrainConverter(),
                    new TileConverter()
                },
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            File.AppendAllText(path, JsonConvert.SerializeObject(builder, settings));
            AssetDatabase.Refresh();
            Debug.Log($"Wrote sample builder to {path}.");
        }

        [MenuItem("Assets/Pantheon/JSON/Sample/World Plan")]
        private static void GenerateSampleWorldPlan()
        {
            TerrainDefinition ground = ScriptableObject.CreateInstance<TerrainDefinition>();
            ground.name = "Terrain_StoneFloor";
            TerrainDefinition wall = ScriptableObject.CreateInstance<TerrainDefinition>();
            wall.name = "Terrain_StoneWall";
            Tile mockTile = ScriptableObject.CreateInstance<Tile>();
            mockTile.name = "StoneStairs_Up";

            Builder builder = new Builder
            {
                ID = "SAMPLE_ID"
            };

            WorldPlan plan = new WorldPlan()
            {
                Builders = new Dictionary<string, Builder>()
            };
            plan.Builders.Add(builder.ID, builder);

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
                    new WorldPlanConverter()
                },
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            File.AppendAllText(path, JsonConvert.SerializeObject(plan, settings));
            AssetDatabase.Refresh();
            Debug.Log($"Wrote sample world plan to {path}.");
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

        [MenuItem("Assets/Pantheon/JSON/Sample/Profession")]
        private static void GenerateSampleProfession()
        {
            Assets.LoadAssets();

            Profession prof = new Profession
            {
                ID = "SAMPLE_ID",
                Name = "SAMPLE_NAME"
            };

            string path = Application.dataPath + $"/Sample/sample_profession.json";

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
                    new TalentConverter(),
                    new TileConverter(),
                    new RuleTileConverter(),
                    new SpeciesDefinitionConverter(),
                    new BodyPartConverter(),
                    new StatusConverter()
                }
            };

            File.AppendAllText(path, JsonConvert.SerializeObject(prof, settings));
            AssetDatabase.Refresh();
            Debug.Log($"Wrote sample template with all components to {path}.");
            Assets.UnloadAssets();
        }

        [MenuItem("Assets/Pantheon/JSON/Sample/Talent")]
        private static void GenerateSampleTalent()
        {
            Assets.LoadAssets();
            Talent talent = new Talent
            {
                ID = "Talent_Foobar",
                Name = "Foobar",
                Icon = Assets.Sprites["Sprite_Punch"],
                OnCast = null,
                PlayerTime = TurnScheduler.TurnTime,
                Behaviour = new CellTalent
                {
                    Accuracy = 75,
                    Range = 1,
                    Damages = new Damage[]
                    {
                        new Damage()
                        {
                            Type = DamageType.Bludgeoning,
                            Min = 2,
                            Max = 4
                        }
                    }
                }
            };

            string path = Application.dataPath + $"/Sample/sample_talent.json";

            if (File.Exists(path))
                File.Delete(path);

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = Binders._entity,
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter>()
                {
                    new AudioClipConverter(),
                    new GameObjectConverter(),
                    new SpriteConverter()
                }
            };

            File.AppendAllText(path, JsonConvert.SerializeObject(talent, settings));
            AssetDatabase.Refresh();
            Assets.UnloadAssets();
            Debug.Log($"Wrote sample body part to {path}.");
        }
    }
}
