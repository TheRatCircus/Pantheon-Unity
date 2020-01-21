// ITurnScheduler.cs
// Jerome Martina

using Pantheon.Components;
using Pantheon.World;
using System;

namespace Pantheon.Core
{
    public interface ITurnScheduler
    {
        void AddActor(Actor actor);
        void RemoveActor(Actor actor);
        void Lock();
        void Unlock();
        void MarkDirtyCell(Cell cell);
        event Action ClockTickEvent;
    }
}
