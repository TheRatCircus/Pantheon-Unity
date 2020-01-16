// Heap.cs
// Courtesy of Sebastian Lague

using System;

namespace Pantheon.Utils
{
    public sealed class Heap<T> where T : IHeapItem<T>
    {
        public T[] Items { get; set; }
        public int CurrentItemCount { get; private set; }

        public Heap(int maxSize) => Items = new T[maxSize];

        public void Add(T item)
        {
            item.HeapIndex = CurrentItemCount;
            Items[CurrentItemCount] = item;
            SortUp(item);
            CurrentItemCount++;
        }

        public T RemoveFirst()
        {
            T firstItem = Items[0];
            CurrentItemCount--;
            Items[0] = Items[CurrentItemCount];
            Items[0].HeapIndex = 0;
            SortDown(Items[0]);
            return firstItem;
        }

        public bool Contains(T item)
        {
            return Equals(Items[item.HeapIndex], item);
        }

        public void UpdateItem(T item)
        {
            SortUp(item);
        }

        private void SortUp(T item)
        {
            int parentIndex = (item.HeapIndex - 1) / 2;
            while (true)
            {
                T parent = Items[parentIndex];

                if (item.CompareTo(parent) > 0)
                    Swap(item, parent);
                else
                    break;

                parentIndex = (item.HeapIndex - 1) / 2;
            }
        }

        private void SortDown(T item)
        {
            while (true)
            {
                int childIndexLeft = item.HeapIndex * 2 + 1;
                int childIndexRight = item.HeapIndex * 2 + 2;
                int swapIndex = 0;

                if (childIndexLeft < CurrentItemCount)
                {
                    swapIndex = childIndexLeft;
                    if (childIndexRight < CurrentItemCount)
                    {
                        if (Items[childIndexLeft].CompareTo(Items[childIndexRight]) < 0)
                            swapIndex = childIndexRight;
                    }

                    if (item.CompareTo(Items[swapIndex]) < 0)
                        Swap(item, Items[swapIndex]);
                    else
                        return;
                }
                else
                {
                    return;
                }
            }
        }

        private void Swap(T itemA, T itemB)
        {
            Items[itemA.HeapIndex] = itemB;
            Items[itemB.HeapIndex] = itemA;

            int temp = itemA.HeapIndex;
            itemA.HeapIndex = itemB.HeapIndex;
            itemB.HeapIndex = temp;
        }
    }

    public interface IHeapItem<T> : IComparable<T>
    {
        int HeapIndex { get; set; }
    }
}
