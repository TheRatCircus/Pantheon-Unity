// NullTurnScheduler.cs
// Jerome Martina

using Pantheon.Components;
using System;

namespace Pantheon.Core
{
    public sealed class NullTurnScheduler : ITurnScheduler
    {
        public event Action ClockTickEvent;
        public void AddActor(Actor actor) { }
        public void Lock() { }
        public void RemoveActor(Actor actor) { }
        public void Unlock() { }
    }
}
