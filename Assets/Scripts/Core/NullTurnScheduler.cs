// NullTurnScheduler.cs
// Jerome Martina

using Pantheon.Components;
using System;
using UnityEngine;

namespace Pantheon.Core
{
    /// <summary>
    /// Dummy turn scheduler for pre-game start.
    /// </summary>
    public sealed class NullTurnScheduler : ITurnScheduler
    {
        public event Action ClockTickEvent { add { } remove { } }
        public void AddActor(Actor actor) { }
        public void Lock() { }
        public void RemoveActor(Actor actor) { }
        public void Unlock() { }
        public void DirtyCell(Vector2Int cell) { }
    }
}
