// ITurnScheduler.cs
// Jerome Martina

using Pantheon.Components;
using System;

namespace Pantheon.Core
{
    public interface ITurnScheduler
    {
        void AddActor(Actor actor);
        void RemoveActor(Actor actor);
        void Lock();
        void Unlock();
        event Action ClockTickEvent;
    }
}
