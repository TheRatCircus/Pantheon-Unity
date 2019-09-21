// Layer.cs
// Jerome Martina

using System;
using System.Collections.Generic;
using UnityEngine;
using Pantheon.Core;
using Pantheon.WorldGen;
using Pantheon.Utils;

namespace Pantheon.World
{
    /// <summary>
    /// A horizontal slice of the game world.
    /// </summary>
    public sealed class Layer
    {
        public int ZLevel { get; }
        public Dictionary<Vector2Int, Level> Levels { get; }
            = new Dictionary<Vector2Int, Level>();

        // If a level is requested at a position and does not exist yet,
        // what should be generated there?
        public Dictionary<Vector2Int, GenerationMap.LevelGenDelegate>
            GenMap { get; }

        public Layer(int zLevel,
            Dictionary<Vector2Int, GenerationMap.LevelGenDelegate> genMap)
        {
            ZLevel = zLevel;
            GenMap = genMap;
        }

        public Level RequestLevel(Vector2Int coords)
        {
            if (!Levels.ContainsKey(coords))
            {
                Level newLevel;

                if (!GenMap.TryGetValue(coords, out GenerationMap.LevelGenDelegate d))
                    throw new ArgumentException("Bad coords given.");

                newLevel = Game.instance.MakeNewLevel();
                d.Invoke(newLevel, new LevelGenArgs
                    (new Vector3Int(coords.x, coords.y, ZLevel), null));
                Levels.Add(coords, newLevel);

                if (Levels.Count > 1)
                    ConnectLevel(newLevel);

                return newLevel;
            }
            else
            {
                Levels.TryGetValue(coords, out Level newLevel);
                return newLevel;
            }
        }

        public void ConnectLevel(Level level)
        {
            UnityEngine.Debug.Log
                ($"Attempting to connect {level.RefName} to its neighbours...");
            if (level.LateralConnections.Count > 0)
            {
                foreach (KeyValuePair<CardinalDirection, Connection> pair
                    in level.LateralConnections)
                {
                    CardinalDirection dir = pair.Key;
                    Connection homeConn = pair.Value;

                    if (!Levels.TryGetValue(level.LayerPos + Helpers.CardinalToV2I(dir),
                        out Level other))
                        continue; // Not generated yet, let it handle itself

                    if (!other.LateralConnections.TryGetValue(Helpers.CardinalOpposite(dir),
                        out Connection otherConn))
                        throw new Exception("Other level has no valid opposite.");

                    homeConn.SetDestination(otherConn);
                }
            }

            if (level.UpConnections.HasElements())
            {
                for (int i = 0; i < level.UpConnections.Length; i++)
                {
                    if (!Game.instance.Layers.TryGetValue(ZLevel + 1,
                        out Layer layerAbove))
                        break;

                    if (!layerAbove.Levels.TryGetValue(level.LayerPos,
                        out Level other))
                        break; // Not generated yet

                    if (level.UpConnections[i] != null)
                    {
                        if (other.DownConnections[i] != null)
                            level.UpConnections[i].SetDestination
                                (other.DownConnections[i]);
                        else
                            throw new Exception("Other has no compatible" +
                                "downwards connection.");
                    }
                }
            }

            if (level.DownConnections.HasElements())
            {
                for (int i = 0; i < level.DownConnections.Length; i++)
                {
                    if (!Game.instance.Layers.TryGetValue(ZLevel - 1,
                        out Layer layerBelow))
                        break;

                    if (!layerBelow.Levels.TryGetValue(level.LayerPos,
                        out Level other))
                        break; // Not generated yet

                    if (level.DownConnections[i] != null)
                    {
                        if (other.UpConnections[i] != null)
                            level.DownConnections[i].SetDestination
                                (other.UpConnections[i]);
                        else
                            throw new Exception("Other has no compatible" +
                                "upwards connection.");
                    }
                }
            }
        }
    }

    /// <summary>
    /// Holds callbacks at given keys for level generation.
    /// </summary>
    public static class GenerationMap
    {
        public delegate void LevelGenDelegate(Level level, LevelGenArgs args);

        public static Dictionary<Vector2Int, LevelGenDelegate> _overworld
            = new Dictionary<Vector2Int, LevelGenDelegate>()
            {
                { new Vector2Int(0, 0), Zones.Valley },
                { new Vector2Int(0, 1), Zones.Valley },
                { new Vector2Int(1, 0), Zones.Valley },
                { new Vector2Int(0, -1), Zones.Valley },
                { new Vector2Int(-1, 0), Zones.Valley }
            };
    }
}