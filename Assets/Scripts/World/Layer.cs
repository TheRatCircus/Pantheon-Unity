// Layer.cs
// Jerome Martina

using Pantheon.Core;
using Pantheon.Utils;
using Pantheon.WorldGen;
using System;
using System.Collections.Generic;
using UnityEngine;

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
        public Dictionary<Vector2Int, LevelBuilder>
            BuilderMap
        { get; } = new Dictionary<Vector2Int, LevelBuilder>();
        
        public Layer(int zLevel)
        {
            ZLevel = zLevel;
            BuilderMap = new Dictionary<Vector2Int, LevelBuilder>();

            Zone valley = new Zone("The Valley", "valley", Vector2Int.zero);
            for (int x = -1; x <= 1; x++)
                for (int y = -1; y <= 1; y++)
                {
                    Vector2Int layerPos = new Vector2Int(x, y);
                    if (x == 0 && y == 0)
                    {
                        BuilderMap.Add(new Vector2Int(x, y),
                            new ValleyBuilder(this, layerPos,
                            valley, CardinalDirection.Centre));
                    }
                    else
                    {
                        CardinalDirection wing = layerPos.ToCardinal();
                        BuilderMap.Add(new Vector2Int(x, y),
                            new ValleyBuilder(this, layerPos, valley, wing));
                    }
                }
            for (int i = 0; i < 10; i++)
                new SurfaceTunneler(this).Start();
        }

        public Level RequestLevel(Vector2Int coords)
        {
            if (!Levels.ContainsKey(coords))
            {
                Level newLevel;

                if (!BuilderMap.TryGetValue(coords,
                    out LevelBuilder builder))
                    throw new ArgumentException("Bad coords given.");
                newLevel = Game.instance.MakeNewLevel();
                builder.Generate(newLevel);
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
                ($"Attempting to connect {level.RefName}" +
                $" to its neighbours...");
            if (level.LateralConnections.Count > 0)
            {
                foreach (KeyValuePair<CardinalDirection, Connection> pair
                    in level.LateralConnections)
                {
                    CardinalDirection dir = pair.Key;
                    Connection homeConn = pair.Value;

                    if (!Levels.TryGetValue(level.LayerPos
                        + Helpers.CardinalToV2I(dir), out Level other))
                        continue; // Not generated yet, let it handle itself

                    if (!other.LateralConnections.TryGetValue(
                        Helpers.CardinalOpposite(dir),
                        out Connection otherConn))
                        throw new Exception
                            ("Other level has no valid opposite.");

                    homeConn.SetDestination(otherConn);
                }
            }

            if (level.UpConnections != null &&
                level.UpConnections.HasElements())
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

            if (level.DownConnections != null &&
                level.DownConnections.HasElements())
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
}