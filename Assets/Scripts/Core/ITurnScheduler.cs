// ITurnScheduler.cs
// Jerome Martina

using Pantheon.Components.Entity;
using Pantheon.World;
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
        void MarkCell(Vector2Int pos);
        void UnmarkCell(Vector2Int pos);
        void RedrawDirtyCells(Level level);
        void SetDirtyCell(Vector2Int cell);
        event Action ClockTickEvent;
    }
}
