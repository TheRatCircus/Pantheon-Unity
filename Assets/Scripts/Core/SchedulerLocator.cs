// SchedulerLocator.cs
// Jerome Martina

namespace Pantheon.Core
{
    public static class SchedulerLocator
    {
        public static ITurnScheduler _scheduler { get; private set; }

        public static void Provide(ITurnScheduler scheduler)
        {
            _scheduler = scheduler;
        }
    }
}
