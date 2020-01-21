// ITurnScheduler.cs
// Jerome Martina

using Pantheon.Components;
using System;
using UnityEngine;

namespace Pantheon.Core
{
    public interface ITurnScheduler
    {
        void AddActor(Actor actor);
        void RemoveActor(Actor actor);
        void Lock();
        void Unlock();
        void DirtyCell(Vector2Int cell);
        event Action ClockTickEvent;
    }
}
