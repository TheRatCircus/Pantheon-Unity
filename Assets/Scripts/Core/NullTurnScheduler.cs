// NullTurnScheduler.cs
// Jerome Martina

using Pantheon.Components.Entity;
using Pantheon.World;
using System;
using UnityEngine;

namespace Pantheon.Core
{
    public sealed class NullTurnScheduler : ITurnScheduler
    {
        public event Action ClockTickEvent { add { } remove { } }
        public void Lock() { }
        public void Unlock() { }
        public void AddActor(Actor actor) { }
        public void RemoveActor(Actor actor) { }
        public void TargetCell(Vector2Int pos) { }
        public void UntargetCell(Vector2Int pos) { }
        public void MarkDirtyCell(Cell cell) { }
    }
}