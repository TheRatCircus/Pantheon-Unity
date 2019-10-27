// Database.cs
// Jerome Martina

using Pantheon.Utils;
using Pantheon.WorldGen;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon
{
    public abstract class Content : ScriptableObject
    {
        [SerializeField] protected string id = "DEFAULT_ID";
        public string ID { get => id; }
        public void SetID() => id = name;
    }
}

namespace Pantheon.Core
{
    /// <summary>
    /// In-game database holding all template data.
    /// </summary>
    public sealed class Database : MonoBehaviour
    {
        private static Database GetDatabase() => Game.instance.Database;

        [SerializeField]
        private List<Content> content = new List<Content>();
        public List<Content> Content => content;
        public Dictionary<string, Content> Dict =
            new Dictionary<string, Content>();

        [SerializeField] private GameObject genericNPC = null;
        public static GameObject GenericNPC => GetDatabase().genericNPC;

        [SerializeField] private Sprite lineTargetOverlay = null;
        public static Sprite LineTargetOverlay
            => GetDatabase().lineTargetOverlay;

        [SerializeField] private GameObject tossFXPrefab = null;
        public static GameObject TossFXPrefab => GetDatabase().tossFXPrefab;

        [SerializeField] private RuleTile splatterTile = null;
        public static RuleTile SplatterTile => GetDatabase().splatterTile;

        [SerializeField] private TextAsset relicNames = null;
        public static TextAsset RelicNames => GetDatabase().relicNames;

        private void Awake()
        {
            foreach (Content c in content)
            {
                Dict.Add(c.ID, c);
            }
        }

        public static bool Contains(string id)
            => GetDatabase().Dict.ContainsKey(id);

        public static T Get<T>(string id) where T : Content
        {
            if (!GetDatabase().Dict.TryGetValue(id, out Content ret))
                throw new ArgumentException($"{id} not found.");

            if (!ret is T)
                throw new ArgumentException(
                    $"Type parameter {typeof(T).ToString()} given:" +
                    $" returned {ret.GetType()}");

            return (T)ret;
        }
    }
}
