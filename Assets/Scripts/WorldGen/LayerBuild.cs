// LayerBuild.cs
// Jerome Martina

using Pantheon.World;
using UnityEngine;
using static Pantheon.Utils.Helpers;
using static Pantheon.Utils.RandomUtils;

namespace Pantheon.WorldGen
{
    public sealed class ZoneTunneler
    {
        private Layer layer;
        private Vector2Int position;

        public ZoneTunneler(Layer layer)
        {
            this.layer = layer;
            position = new Vector2Int();
        }

        public void Start() => Move(10);

        private void Move(int maxMoves)
        {
            for (int moves = 0; moves < maxMoves; moves++)
            {
                Vector2Int delta = new Vector2Int();
                // Start with a random direction
                if (CoinFlip(true)) // Move horizontally
                    delta.x = CoinFlip(true) ? 4 : -4;
                else                // Move vertically
                    delta.y = CoinFlip(true) ? 4 : -4;

                /// EXPLANATION
                /// 
                /// Four points are relevant when tunneling:
                /// - position, the start before tunneling
                /// - preTransition, for a ConnectionCallback
                /// - postTransition, for another ConnectionCallback
                /// - destination, around which to build a zone
                /// 
                /// ### P = position 
                /// #D# ^ = preTransition
                /// #X# D = destination
                ///  |  X = postTransition
                /// #^#
                /// #P#
                /// ###
                ///
                
                Vector2Int transition = position + delta.Quotient(2);
                Vector2Int destination = position + delta;

                // Is there a transition to the destination already?
                if (layer.BuilderMap.ContainsKey(transition))
                {
                    position = destination;
                    continue; // Move to destination and do nothing else
                }
                else if (layer.BuilderMap.ContainsKey(destination))
                {
                    // Only build transition
                    Vector2Int preTransition
                        = position + delta.Quotient(4);
                    Vector2Int postTransition
                        = destination - delta.Quotient(4);

                    position = preTransition;
                    layer.BuilderMap.TryGetValue(position,
                        out LevelBuilder preTransitionBuilder);
                    ZoneLevelBuilder pre = preTransitionBuilder
                        as ZoneLevelBuilder;

                    pre.ZoneConnect = Connect.ConnectByDirection;

                    position = transition;
                    BuildTransition(delta);

                    position = postTransition;
                    layer.BuilderMap.TryGetValue(position,
                        out LevelBuilder postTransitionBuilder);
                    ZoneLevelBuilder post = postTransitionBuilder
                        as ZoneLevelBuilder;
                    post.ZoneConnect = Connect.ConnectByDirection;

                    position = destination; // Now we can tunnel again
                }
                else // Something needs to be put at destination
                {
                    Vector2Int preTransition
                        = position + delta.Quotient(4);
                    Vector2Int postTransition
                        = destination - delta.Quotient(4);

                    position = preTransition;
                    layer.BuilderMap.TryGetValue(position,
                        out LevelBuilder preTransitionBuilder);
                    ZoneLevelBuilder pre = preTransitionBuilder
                        as ZoneLevelBuilder;

                    pre.ZoneConnect = Connect.ConnectByDirection;

                    position = transition;
                    BuildTransition(delta);

                    position = destination;
                    BuildZone(delta);

                    position = postTransition;
                    layer.BuilderMap.TryGetValue(position,
                        out LevelBuilder postTransitionBuilder);
                    ZoneLevelBuilder post = postTransitionBuilder
                        as ZoneLevelBuilder;
                    post.ZoneConnect = Connect.ConnectByDirection;

                    position = destination; // Now we can tunnel again
                }
            }
        }

        private void BuildTransition(Vector2Int delta)
        {
            // To create a transition level, both the origin
            // and destination level themes need to be known
            CardinalDirection end = delta.ToCardinal();
            CardinalDirection start = CardinalOpposite(end);
            layer.BuilderMap.Add(position,
                new TransitionBuilder(layer, position, start, end));
        }

        private void BuildZone(Vector2Int delta)
        {
            ZoneTheme theme = ThemeDefs._valley;
            Zone zone = new Zone($"{theme.DisplayName} of Ghoti",
                $"{theme.ThemeRef}_Ghoti", position);
            for (int x = position.x - 1; x <= position.x + 1; x++)
                for (int y = position.y - 1; y <= position.y + 1; y++)
                {
                    Vector2Int layerPos = new Vector2Int(x, y);
                    Vector2Int zonePos = 
                        new Vector2Int(x - position.x, y - position.y);
                    CardinalDirection wing = zonePos.ToCardinal();
                    layer.BuilderMap.Add(layerPos,
                        new ZoneLevelBuilder(layer, layerPos, zone, zonePos,
                        wing, theme));
                }
        }
    }
}
