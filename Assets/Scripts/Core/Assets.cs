// Assets.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon.Components.Entity;
using Pantheon.Content;
using Pantheon.Gen;
using Pantheon.Serialization.Json;
using Pantheon.Serialization.Json.Converters;
using Pantheon.Utils;
using Pantheon.World;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Pantheon
{
    public static class Assets
    {
        [RuntimeInitializeOnLoadMethod]
        public static void LoadAssets()
#pragma warning restore IDE0051
        {
            bundleMain = AssetBundle.LoadFromFile(Path.Combine(
                Application.streamingAssetsPath, "pantheon"));
            bundleTemplates = AssetBundle.LoadFromFile(Path.Combine(
                Application.streamingAssetsPath, "pantheon_templates"));
            bundleTalents = AssetBundle.LoadFromFile(Path.Combine(
                Application.streamingAssetsPath, "pantheon_talents"));
            bundleSpecies = AssetBundle.LoadFromFile(Path.Combine
                (Application.streamingAssetsPath, "pantheon_species"));
            bundleBody = AssetBundle.LoadFromFile(Path.Combine(
                Application.streamingAssetsPath, "pantheon_body"));
            bundleBuilders = AssetBundle.LoadFromFile(Path.Combine(
                Application.streamingAssetsPath, "pantheon_builders"));
            bundleProfessions = AssetBundle.LoadFromFile(Path.Combine(
                Application.streamingAssetsPath, "pantheon_professions"));
            bundleAI = AssetBundle.LoadFromFile(Path.Combine(
                Application.streamingAssetsPath, "pantheon_ai"));

            Object[] objs = bundleMain.LoadAllAssets();
            Object[] templateFiles = bundleTemplates.LoadAllAssets();
            Object[] aiFiles = bundleAI.LoadAllAssets();
            Object[] profFiles = bundleProfessions.LoadAllAssets();
            Object[] talentFiles = bundleTalents.LoadAllAssets();
            Object[] speciesFiles = bundleSpecies.LoadAllAssets();
            Object[] bodyFiles = bundleBody.LoadAllAssets();
            Object[] builderFiles = bundleBuilders.LoadAllAssets();

            _prefabs = new Dictionary<string, GameObject>();
            Prefabs = new ReadOnlyDictionary<string, GameObject>(_prefabs);
            _templates = new Dictionary<string, EntityTemplate>(templateFiles.Length);
            _corpses = new Dictionary<string, EntityTemplate>();
            _ai = new Dictionary<string, AIDefinition>(aiFiles.Length);
            AI = new ReadOnlyDictionary<string, AIDefinition>(_ai);
            _professions = new Dictionary<string, Profession>(profFiles.Length);
            _sprites = new Dictionary<string, Sprite>();
            Sprites = new ReadOnlyDictionary<string, Sprite>(_sprites);
            _tiles = new Dictionary<string, TileBase>();
            _audio = new Dictionary<string, AudioClip>();
            Audio = new ReadOnlyDictionary<string, AudioClip>(_audio);
            _talents = new Dictionary<string, Talent>(talentFiles.Length);
            Talents = new ReadOnlyDictionary<string, Talent>(_talents);
            _species = new Dictionary<string, SpeciesDefinition>(speciesFiles.Length);
            Species = new ReadOnlyDictionary<string, SpeciesDefinition>(_species);
            _vaults = new Dictionary<string, Vault>();
            Vaults = new ReadOnlyDictionary<string, Vault>(_vaults);
            _bodyParts = new Dictionary<string, BodyPart>(bodyFiles.Length);
            BodyParts = new ReadOnlyDictionary<string, BodyPart>(_bodyParts);
            _builders = new Dictionary<string, Builder>(builderFiles.Length);
            Builders = new ReadOnlyDictionary<string, Builder>(_builders);
            _dialogue = new Dictionary<string, TextAsset>();
            Dialogue = new ReadOnlyDictionary<string, TextAsset>(_dialogue);

            int t = 1; // 0 represents no terrain

            TextAsset worldPlanFile = null;

            foreach (Object obj in objs)
            {
                if (obj is Sprite sprite)
                {
                    _sprites.Add(sprite.name, sprite);
                    continue;
                }

                if (obj is TileBase tile)
                {
                    _tiles.Add(tile.name, tile);
                    continue;
                }

                if (obj is AudioClip audio)
                {
                    _audio.Add(audio.name, audio);
                    continue;
                }

                if (obj is TerrainDefinition td)
                {
                    TerrainIDs[t] = td.name;
                    Terrains[t++] = td;
                    continue;
                }

                if (obj is Vault vault)
                {
                    _vaults.Add(vault.name, vault);
                    continue;
                }

                if (obj is GameObject prefab)
                {
                    switch (prefab.name)
                    {
                        default:
                            _prefabs.Add(prefab.name, prefab);
                            continue;
                        case "FX_Toss":
                            TossFXPrefab = prefab;
                            continue;
                    }
                }

                if (obj is TextAsset ta)
                {
                    switch (ta.name)
                    {
                        case "WorldPlan":
                            worldPlanFile = ta;
                            continue;
                        case "CharacterNames":
                            CharacterNames = ta;
                            continue;
                        case "RelicNames":
                            RelicNames = ta;
                            continue;
                        default:
                            break;
                    }

                    string[] tokens = ta.name.Split('_');

                    switch (tokens[0])
                    {
                        case "Dialogue":
                            _dialogue.Add(tokens[1], ta);
                            break;
                        default:
                            throw new System.Exception(
                                $"Unrecognized TextAsset \"{ta.name}\"");
                    }
                }
            }

            foreach (TextAsset ta in talentFiles)
            {
                Talent talent =
                    JsonConvert.DeserializeObject<Talent>(
                        ta.text, _talentSettings);
                _talents.Add(talent.ID, talent);
            }

            foreach (TextAsset ta in aiFiles)
            {
                AIDefinition def =
                    JsonConvert.DeserializeObject<AIDefinition>(
                        ta.text, _aiSettings);
                _ai.Add(def.ID, def);
            }

            foreach (TextAsset ta in bodyFiles)
            {
                BodyPart part = JsonConvert.DeserializeObject<BodyPart>(
                    ta.text, _partSettings);
                _bodyParts.Add(part.ID, part);
            }

            foreach (TextAsset ta in speciesFiles)
            {
                SpeciesDefinition s =
                    JsonConvert.DeserializeObject<SpeciesDefinition>(
                        ta.text, _speciesSettings);
                _species.Add(s.ID, s);
            }

            foreach (TextAsset ta in builderFiles)
            {
                Builder builder = JsonConvert.DeserializeObject<Builder>(
                    ta.text, _builderSettings);
                _builders.Add(builder.ID, builder);
            }

            WorldPlan = JsonConvert.DeserializeObject<WorldPlan>(
                                worldPlanFile.text,
                                _builderSettings);

            // Template, profession data is lazy-initialized
        }

        public static void UnloadAssets()
        {
            AssetBundle.UnloadAllAssetBundles(true);
        }

        private static AssetBundle bundleMain;
        private static AssetBundle bundleTemplates;
        private static AssetBundle bundleAI;
        private static AssetBundle bundleTalents;
        private static AssetBundle bundleSpecies;
        private static AssetBundle bundleBody;
        private static AssetBundle bundleBuilders;
        private static AssetBundle bundleProfessions;

        private static readonly JsonSerializerSettings _genericSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            SerializationBinder = Binders._entity,
            Formatting = Formatting.Indented,
            Converters = new List<JsonConverter>()
                {
                    new AIDefinitionConverter(),
                    new AudioClipConverter(),
                    new BodyPartConverter(),
                    new GameObjectConverter(),
                    new RuleTileConverter(),
                    new SpeciesDefinitionConverter(),
                    new SpriteConverter(),
                    new StatusConverter(),
                    new TalentConverter(),
                    //new TemplateArrayConverter(),
                    new TileConverter(),
                }
        };

        private static readonly JsonSerializerSettings _aiSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            SerializationBinder = Binders._entity,
            Formatting = Formatting.Indented
        };

        private static readonly JsonSerializerSettings _talentSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            SerializationBinder = Binders._entity,
            Formatting = Formatting.Indented,
            Converters = new List<JsonConverter>()
                {
                    new AudioClipConverter(),
                    new GameObjectConverter(),
                    new SpriteConverter(),
                    new StatusConverter()
                }
        };

        private static readonly JsonSerializerSettings _builderSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            SerializationBinder = Binders._builder,
            Formatting = Formatting.Indented,
            Converters = new List<JsonConverter>()
                {
                    new WorldPlanConverter(),
                    new RuleTileConverter(),
                    new TerrainConverter(),
                    new TileConverter()
                }
        };

        private static readonly JsonSerializerSettings _partSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            SerializationBinder = Binders._entity,
            Formatting = Formatting.Indented,
            Converters = new List<JsonConverter>()
                {
                    new SpriteConverter()
                }
        };

        private static readonly JsonSerializerSettings _speciesSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            SerializationBinder = Binders._entity,
            Formatting = Formatting.Indented,
            Converters = new List<JsonConverter>()
                {
                    new BodyPartConverter(),
                    new SpriteConverter()
                }
        };

        // Misc

        public static TextAsset CharacterNames { get; private set; }
        public static TextAsset RelicNames { get; private set; }
        public static WorldPlan WorldPlan { get; private set; }

        // Prefabs

        private static Dictionary<string, GameObject> _prefabs;
        public static ReadOnlyDictionary<string, GameObject> Prefabs
        { get; private set; }

        public static GameObject TossFXPrefab { get; private set; }

        // Templates

        private static Dictionary<string, EntityTemplate> _templates;

        public static EntityTemplate GetTemplate(string id)
        {
            if (_templates.TryGetValue(id, out EntityTemplate ret))
                return ret;
            else
            {
                TextAsset ta = bundleTemplates.LoadAsset<TextAsset>(id);

                if (ta == null)
                    return null;

                EntityTemplate template =
                    JsonConvert.DeserializeObject<EntityTemplate>(
                        ta.text, _genericSettings);
                _templates.Add(template.ID, template);
                return template;
            }
        }

        public static bool TemplateExists(string id) => _templates.ContainsKey(id);

        // Corpse flyweights

        private static Dictionary<string, EntityTemplate> _corpses;

        public static EntityTemplate GetCorpseTemplate(Entity entity)
        {
            // TODO: Handle entities which leave corpses but have no species
            string speciesName = 
                entity.GetComponent<Species>().
                SpeciesDef.Name.FirstCharToUpper();
            string id = $"Corpse_{speciesName}";
            if (_corpses.TryGetValue(id, out EntityTemplate ret))
                return ret;
            else
            {
                EntityTemplate template = EntityTemplate.Corpse(id, speciesName, entity);
                _corpses.Add(template.ID, template);
                return template;
            }
        }

        // AI

        private static Dictionary<string, AIDefinition> _ai;
        public static ReadOnlyDictionary<string, AIDefinition> AI { get; private set; }

        // Professions

        private static Dictionary<string, Profession> _professions;

        public static Profession GetProfession(string id)
        {
            if (_professions.TryGetValue(id, out Profession ret))
                return ret;
            else
            {
                TextAsset ta = bundleProfessions.LoadAsset<TextAsset>(id);

                if (ta == null)
                    return null;

                Profession prof =
                    JsonConvert.DeserializeObject<Profession>(
                        ta.text, _genericSettings);
                _professions.Add(prof.ID, prof);
                return prof;
            }
        }

        // Sprites

        private static Dictionary<string, Sprite> _sprites;
        public static ReadOnlyDictionary<string, Sprite> Sprites
        { get; private set; }

        // Tiles

        private static Dictionary<string, TileBase> _tiles;

        /// <summary>
        /// Get the tile corresponding to a sprite.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sprite"></param>
        /// <returns></returns>
        public static T GetTile<T>(Sprite sprite) where T : TileBase
        {
            // Sprite_Foobar becomes Tile_Foobar
            string id = $"Tile_{sprite.name.Split('_')[1]}";

            if (_tiles.TryGetValue(id, out TileBase ret))
                return (T)ret;
            else
            {
                T tile = ScriptableObject.CreateInstance<T>();
                tile.name = id;

                if (tile is Tile t)
                    t.sprite = sprite;
                else if (tile is RuleTile rt)
                    rt.m_DefaultSprite = sprite;
                
                return tile;
            }
        }

        public static T GetTile<T>(string id) where T : TileBase
        {
            if (_tiles.TryGetValue(id, out TileBase ret))
                return (T)ret;
            else
                return null;
        }

        // AudioClips

        private static Dictionary<string, AudioClip> _audio;
        public static ReadOnlyDictionary<string, AudioClip> Audio
        { get; private set; }

        // Terrain

        public static string[] TerrainIDs { get; private set; }
            = new string[8];
        public static TerrainDefinition[] Terrains { get; private set; }
            = new TerrainDefinition[8];

        public static TerrainDefinition GetTerrain(string id)
        {
            int i = 0;
            foreach (string s in TerrainIDs)
            {
                if (TerrainIDs[i] == id)
                    break;

                i++;
            }
            return Terrains[i];
        }

        public static TerrainDefinition GetTerrain(byte index)
        {
            return Terrains[index];
        }

        public static byte GetTerrainIndex(TerrainDefinition terrain)
        {
            for (byte i = 0; i < Terrains.Length; i++)
            {
                if (Terrains[i] == terrain)
                    return i;
            }
            throw new System.Exception(
                $"{terrain} not found in asset database.");
        }

        // Talents

        private static Dictionary<string, Talent> _talents;
        public static ReadOnlyDictionary<string, Talent> Talents
        { get; private set; }

        // Species

        private static Dictionary<string, SpeciesDefinition> _species;
        public static ReadOnlyDictionary<string, SpeciesDefinition> Species
        { get; private set; }

        // Vaults

        private static Dictionary<string, Vault> _vaults;
        public static ReadOnlyDictionary<string, Vault> Vaults
        { get; private set; }

        // Body parts

        private static Dictionary<string, BodyPart> _bodyParts;
        public static ReadOnlyDictionary<string, BodyPart> BodyParts
        { get; private set; }

        // Builder plans

        private static Dictionary<string, Builder> _builders;
        public static ReadOnlyDictionary<string, Builder> Builders
        { get; private set; }

        // Text

        private static Dictionary<string, TextAsset> _dialogue;
        public static ReadOnlyDictionary<string, TextAsset> Dialogue;
    }
}
