// NullTurnScheduler.cs
// Jerome Martina

using Pantheon.Components.Entity;
using Pantheon.World;
using System;

namespace Pantheon.Core
{
    public sealed class NullTurnScheduler : ITurnScheduler
    {
        public event Action ClockTickEvent { add { } remove { } }
        public void AddActor(Actor actor) { }
        public void Lock() { }
        public void RemoveActor(Actor actor) { }
        public void Unlock() { }
        public void MarkDirtyCell(Cell cell) { }
    }
}