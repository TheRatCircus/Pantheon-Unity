// ITurnScheduler.cs
// Jerome Martina

using Pantheon.Components;

namespace Pantheon.Core
{
    public interface ITurnScheduler
    {
        void AddActor(Actor actor);
        void RemoveActor(Actor actor);
        void Lock();
        void Unlock();
    }
}
