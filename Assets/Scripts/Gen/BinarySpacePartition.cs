// BinarySpacePartition.cs
// Courtesy of Timothy Hely

using Newtonsoft.Json;
using Pantheon.Content;
using Pantheon.World;
using System.Collections.Generic;
using UnityEngine;
using static Pantheon.Utils.RandomUtils;

namespace Pantheon.Gen
{
    public sealed class BinarySpacePartition : BuilderStep
    {
        [JsonProperty] private int minRoomSize = 10;
        [JsonProperty] private bool tightFill;
        // Terrain with which to fill rooms
        [JsonProperty] private TerrainDefinition terrain = default;

        public BinarySpacePartition(string terrainID, int minRoomSize, bool tightFill)
        {
            terrain = ScriptableObject.CreateInstance<TerrainDefinition>();
            terrain.name = terrainID;
            this.minRoomSize = minRoomSize;
            this.tightFill = tightFill;
        }

        [JsonConstructor]
        public BinarySpacePartition(TerrainDefinition terrain, int minRoomSize, bool tightFill)
        {
            this.terrain = terrain;
            this.minRoomSize = minRoomSize;
            this.tightFill = tightFill;
        }

        public override void Run(Level level)
        {
            List<Leaf> leaves = new List<Leaf>();

            Leaf root = new Leaf(0, 0, level.Size.x, level.Size.y);
            leaves.Add(root);

            bool didSplit = true;
            while (didSplit)
            {
                didSplit = false;
                for (int i = leaves.Count - 1; i >= 0; i--)
                {
                    if (leaves[i].LeftChild == null &&
                        leaves[i].RightChild == null)
                    {
                        if (leaves[i].Width > Leaf.MaxLeafSize ||
                            leaves[i].Height > Leaf.MaxLeafSize ||
                            OneChanceIn(4))
                        {
                            if (leaves[i].Split())
                            {
                                leaves.Add(leaves[i].LeftChild);
                                leaves.Add(leaves[i].RightChild);
                                didSplit = true;
                            }
                        }
                    }
                }
            }
            root.CreateRooms(minRoomSize, tightFill);
            for (int i = 0; i < leaves.Count; i++)
            {
                if (leaves[i].Room != null)
                    Utils.FillRectTerrain(level, leaves[i].Room, terrain);

                if (leaves[i].Halls != null)
                    foreach (LevelRect hall in leaves[i].Halls)
                    {
                        LevelRect rect = new LevelRect(
                            new Vector2Int(hall.x1, hall.y1),
                            new Vector2Int(
                                hall.x2 - hall.x1 + 2,
                                hall.y2 - hall.y1 + 2));
                        Utils.FillRectTerrain(level, hall, terrain);
                    }
            }
        }

        public override string ToString()
        {
            return $"BSP: Min room size {minRoomSize}, fill terrain: {terrain}";
        }

        /// <summary>
        /// An abstract section of a map used by Binary Space Partitioning.
        /// </summary>
        sealed class Leaf
        {
            public const int MaxLeafSize = 21;
            public const int MinLeafSize = 7;

            public int X { get; set; }
            public int Y { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }

            public Leaf LeftChild { get; set; }
            public Leaf RightChild { get; set; }
            public LevelRect Room { get; set; }
            public Stack<LevelRect> Halls { get; set; }

            public Leaf(int x, int y, int width, int height)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
            }

            public bool Split()
            {
                // Begin splitting the leaf into two children
                if (LeftChild != null || RightChild != null)
                    return false; // Already split

                bool splitH = CoinFlip();
                if (Width > Height && Width / Height >= 1.25f)
                    splitH = false;
                else if (Height > Width && Height / Width >= 1.25f)
                    splitH = true;

                int max = (splitH ? Height : Width) - MinLeafSize;
                if (max <= MinLeafSize)
                    return false; // Area is too small to split again

                int split = RangeInclusive(MinLeafSize, max);

                if (splitH)
                {
                    LeftChild = new Leaf(X, Y, Width, split);
                    RightChild = new Leaf(X, Y + split, Width, Height - split);
                }
                else
                {
                    LeftChild = new Leaf(X, Y, split, Height);
                    RightChild = new Leaf(X + split, Y, Width - split, Height);
                }
                return true;
            }

            /// <summary>
            /// Generates all rooms and hallways for this Leaf and all its children.
            /// </summary>
            public void CreateRooms(int minRoomSize, bool tightFill)
            {
                if (LeftChild != null || RightChild != null)
                {
                    if (LeftChild != null)
                        LeftChild.CreateRooms(minRoomSize, tightFill);
                    if (RightChild != null)
                        RightChild.CreateRooms(minRoomSize, tightFill);

                    if (LeftChild != null && RightChild != null)
                        CreateHall(LeftChild.GetRoom(), RightChild.GetRoom());
                }
                else
                {
                    Vector2Int roomSize;
                    Vector2Int roomPos;

                    if (tightFill)
                        roomSize = new Vector2Int(Width - 1, Height - 1);
                    else
                        roomSize = new Vector2Int(
                            RangeInclusive(minRoomSize, Width - 2),
                            RangeInclusive(minRoomSize, Height - 2));
                    
                    roomPos = new Vector2Int(
                        RangeInclusive(1, Width - roomSize.x - 1),
                        RangeInclusive(1, Height - roomSize.y - 1));
                    Room = new LevelRect(
                        new Vector2Int(X + roomPos.x, Y + roomPos.y),
                        new Vector2Int(roomSize.x, roomSize.y));
                }
            }

            public LevelRect GetRoom()
            {
                if (Room != null)
                    return Room;
                else
                {
                    LevelRect lRoom = null;
                    LevelRect rRoom = null;

                    if (LeftChild != null)
                        lRoom = LeftChild.GetRoom();
                    if (RightChild != null)
                        rRoom = RightChild.GetRoom();

                    if (lRoom == null && rRoom == null)
                        return null;
                    else if (rRoom == null)
                        return lRoom;
                    else if (lRoom == null)
                        return rRoom;
                    else if (OneChanceIn(2))
                        return lRoom;
                    else
                        return rRoom;
                }
            }

            public void CreateHall(LevelRect l, LevelRect r)
            {
                Halls = new Stack<LevelRect>();

                Vector2Int p1 = new Vector2Int(
                    RangeInclusive(l.x1 + 1, l.x2 - 2),
                    RangeInclusive(l.y1 + 1, l.y2 - 2));
                Vector2Int p2 = new Vector2Int(
                    RangeInclusive(r.x1 + 1, r.x2 - 2),
                    RangeInclusive(r.y1 + 1, r.y2 - 2));

                int w = p2.x - p1.x;
                int h = p2.y - p1.y;

                if (w < 0)
                {
                    if (h < 0)
                    {
                        if (CoinFlip())
                        {
                            Halls.Push(new LevelRect(
                                new Vector2Int(p2.x, p1.y),
                                new Vector2Int(Mathf.Abs(w), 1)));
                            Halls.Push(new LevelRect(
                                new Vector2Int(p2.x, p2.y),
                                new Vector2Int(1, Mathf.Abs(h))));
                        }
                        else
                        {
                            Halls.Push(new LevelRect(
                                new Vector2Int(p2.x, p2.y),
                                new Vector2Int(Mathf.Abs(w), 1)));
                            Halls.Push(new LevelRect(
                                new Vector2Int(p1.x, p2.y),
                                new Vector2Int(1, Mathf.Abs(h))));
                        }
                    }
                    else if (h > 0)
                    {
                        if (CoinFlip())
                        {
                            Halls.Push(new LevelRect(
                                new Vector2Int(p2.x, p1.y),
                                new Vector2Int(Mathf.Abs(w), 1)));
                            Halls.Push(new LevelRect(
                                new Vector2Int(p2.x, p1.y),
                                new Vector2Int(1, Mathf.Abs(h))));
                        }
                        else
                        {
                            Halls.Push(new LevelRect(
                                new Vector2Int(p2.x, p2.y),
                                new Vector2Int(Mathf.Abs(w), 1)));
                            Halls.Push(new LevelRect(
                                new Vector2Int(p1.x, p1.y),
                                new Vector2Int(1, Mathf.Abs(h))));
                        }
                    }
                    else // if (h == 0)
                        Halls.Push(new LevelRect(
                            new Vector2Int(p2.x, p2.y),
                            new Vector2Int(Mathf.Abs(w), 1)));
                }
                else if (w > 0)
                {
                    if (h < 0)
                    {
                        if (CoinFlip())
                        {
                            Halls.Push(new LevelRect(
                                new Vector2Int(p1.x, p2.y),
                                new Vector2Int(Mathf.Abs(w), 1)));
                            Halls.Push(new LevelRect(
                                new Vector2Int(p1.x, p2.y),
                                 new Vector2Int(1, Mathf.Abs(h))));
                        }
                        else
                        {
                            Halls.Push(new LevelRect(
                                new Vector2Int(p1.x, p1.y),
                                new Vector2Int(Mathf.Abs(w), 1)));
                            Halls.Push(new LevelRect(
                                new Vector2Int(p2.x, p2.y),
                                new Vector2Int(1, Mathf.Abs(h))));
                        }
                    }
                    else if (h > 0)
                    {
                        if (CoinFlip())
                        {
                            Halls.Push(new LevelRect(
                                new Vector2Int(p1.x, p1.y),
                                new Vector2Int(Mathf.Abs(w), 1)));
                            Halls.Push(new LevelRect(
                                new Vector2Int(p2.x, p1.y),
                                 new Vector2Int(1, Mathf.Abs(h))));
                        }
                        else
                        {
                            Halls.Push(new LevelRect(
                                new Vector2Int(p1.x, p2.y),
                                new Vector2Int(Mathf.Abs(w), 1)));
                            Halls.Push(new LevelRect(
                                new Vector2Int(p1.x, p1.y),
                                 new Vector2Int(1, Mathf.Abs(h))));
                        }
                    }
                    else // if (h == 0)
                        Halls.Push(new LevelRect(
                            new Vector2Int(p1.x, p1.y),
                            new Vector2Int(Mathf.Abs(w), 1)));
                }
                else // if (w == 0)
                {
                    if (h < 0)
                        Halls.Push(new LevelRect(
                            new Vector2Int(p2.x, p2.y),
                            new Vector2Int(1, Mathf.Abs(h))));
                    else if (h > 0)
                        Halls.Push(new LevelRect(
                            new Vector2Int(p1.x, p1.y),
                            new Vector2Int(1, Mathf.Abs(h))));
                }
            }
        }
    }
}
