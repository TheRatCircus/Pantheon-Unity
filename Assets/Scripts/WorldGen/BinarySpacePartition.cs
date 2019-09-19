// BinarySpacePartition.cs
// Credit to Timothy Hely

using System.Collections.Generic;
using UnityEngine;
using Pantheon.World;
using static Pantheon.Utils.RandomUtils;
using static Pantheon.WorldGen.Layout;

namespace Pantheon.WorldGen
{
    public static class BinarySpacePartition
    {
        public static void BSP(Level level, TerrainType terrain, int minRoomSize)
        {
            List<Leaf> leaves = new List<Leaf>();

            Leaf root = new Leaf(0, 0, level.LevelSize.x, level.LevelSize.y);
            leaves.Add(root);

            bool didSplit = true;
            while (didSplit)
            {
                didSplit = false;
                for (int i = leaves.Count - 1; i >= 0; i--)
                {
                    if (leaves[i].LeftChild == null && leaves[i].RightChild == null)
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
            root.CreateRooms(minRoomSize);
            for (int i = 0; i < leaves.Count; i++)
            {
                if (leaves[i].Room != null)
                    FillRect(level, leaves[i].Room, terrain);
                
                if (leaves[i].Halls != null)
                    foreach (Rectangle hall in leaves[i].Halls)
                    {
                        Rectangle rect = new Rectangle(
                            new Vector2Int(hall.x1, hall.y1),
                            new Vector2Int(hall.x2 - hall.x1 + 2, hall.y2 - hall.y1 + 2));
                        FillRect(level, hall, terrain);
                    }
            }                
        }
    }

    /// <summary>
    /// An abstract section of a map used by Binary Space Partitioning.
    /// </summary>
    public class Leaf
    {
        public const int MaxLeafSize = 21;
        public const int MinLeafSize = 13;

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Leaf LeftChild { get; set; }
        public Leaf RightChild { get; set; }
        public Rectangle Room { get; set; }
        public Stack<Rectangle> Halls { get; set; }

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
        public void CreateRooms(int minRoomSize)
        {
            if (LeftChild != null || RightChild != null)
            {
                if (LeftChild != null)
                    LeftChild.CreateRooms(minRoomSize);
                if (RightChild != null)
                    RightChild.CreateRooms(minRoomSize);

                if (LeftChild != null && RightChild != null)
                    CreateHall(LeftChild.GetRoom(), RightChild.GetRoom());
            }
            else
            {
                Vector2Int roomSize;
                Vector2Int roomPos;

                roomSize = new Vector2Int(
                    RangeInclusive(minRoomSize, Width - 2),
                    RangeInclusive(minRoomSize, Height - 2));
                roomPos = new Vector2Int(
                    RangeInclusive(1, Width - roomSize.x - 1),
                    RangeInclusive(1, Height - roomSize.y - 1));
                Room = new Rectangle(
                    new Vector2Int(X + roomPos.x, Y + roomPos.y),
                    new Vector2Int(roomSize.x, roomSize.y));
            }
        }

        public Rectangle GetRoom()
        {
            if (Room != null)
                return Room;
            else
            {
                Rectangle lRoom = null;
                Rectangle rRoom = null;

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

        public void CreateHall(Rectangle l, Rectangle r)
        {
            Halls = new Stack<Rectangle>();

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
                        Halls.Push(new Rectangle(
                            new Vector2Int(p2.x, p1.y),
                            new Vector2Int(Mathf.Abs(w), 1)));
                        Halls.Push(new Rectangle(
                            new Vector2Int(p2.x, p2.y),
                            new Vector2Int(1, Mathf.Abs(h))));
                    }
                    else
                    {
                        Halls.Push(new Rectangle(
                            new Vector2Int(p2.x, p2.y),
                            new Vector2Int(Mathf.Abs(w), 1)));
                        Halls.Push(new Rectangle(
                            new Vector2Int(p1.x, p2.y),
                            new Vector2Int(1, Mathf.Abs(h))));
                    }
                }
                else if (h > 0)
                {
                    if (CoinFlip())
                    {
                        Halls.Push(new Rectangle(
                            new Vector2Int(p2.x, p1.y),
                            new Vector2Int(Mathf.Abs(w), 1)));
                        Halls.Push(new Rectangle(
                            new Vector2Int(p2.x, p1.y),
                            new Vector2Int(1, Mathf.Abs(h))));
                    }
                    else
                    {
                        Halls.Push(new Rectangle(
                            new Vector2Int(p2.x, p2.y),
                            new Vector2Int(Mathf.Abs(w), 1)));
                        Halls.Push(new Rectangle(
                            new Vector2Int(p1.x, p1.y),
                            new Vector2Int(1, Mathf.Abs(h))));
                    }
                }
                else // if (h == 0)
                    Halls.Push(new Rectangle(
                        new Vector2Int(p2.x, p2.y),
                        new Vector2Int(Mathf.Abs(w), 1)));
            }
            else if (w > 0)
            {
                if (h < 0)
                {
                    if (CoinFlip())
                    {
                        Halls.Push(new Rectangle(
                            new Vector2Int(p1.x, p2.y),
                            new Vector2Int(Mathf.Abs(w), 1)));
                        Halls.Push(new Rectangle(
                            new Vector2Int(p1.x, p2.y),
                             new Vector2Int(1, Mathf.Abs(h))));
                    }
                    else
                    {
                        Halls.Push(new Rectangle(
                            new Vector2Int(p1.x, p1.y),
                            new Vector2Int(Mathf.Abs(w), 1)));
                        Halls.Push(new Rectangle(
                            new Vector2Int(p2.x, p2.y),
                            new Vector2Int(1, Mathf.Abs(h))));
                    }
                }
                else if (h > 0)
                {
                    if (CoinFlip())
                    {
                        Halls.Push(new Rectangle(
                            new Vector2Int(p1.x, p1.y),
                            new Vector2Int(Mathf.Abs(w), 1)));
                        Halls.Push(new Rectangle(
                            new Vector2Int(p2.x, p1.y),
                             new Vector2Int(1, Mathf.Abs(h))));
                    }
                    else
                    {
                        Halls.Push(new Rectangle(
                            new Vector2Int(p1.x, p2.y),
                            new Vector2Int(Mathf.Abs(w), 1)));
                        Halls.Push(new Rectangle(
                            new Vector2Int(p1.x, p1.y),
                             new Vector2Int(1, Mathf.Abs(h))));
                    }
                }
                else // if (h == 0)
                    Halls.Push(new Rectangle(
                        new Vector2Int(p1.x, p1.y),
                        new Vector2Int(Mathf.Abs(w), 1)));
            }
            else // if (w == 0)
            {
                if (h < 0)
                    Halls.Push(new Rectangle(
                        new Vector2Int(p2.x, p2.y),
                        new Vector2Int(1, Mathf.Abs(h))));
                else if (h > 0)
                    Halls.Push(new Rectangle(
                        new Vector2Int(p1.x, p1.y),
                        new Vector2Int(1, Mathf.Abs(h))));
            }
        }
    }
}