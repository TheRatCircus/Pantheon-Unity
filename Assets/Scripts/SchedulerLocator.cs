// SchedulerLocator.cs
// Jerome Martina

using Pantheon.Core;

namespace Pantheon
{
    public static class SchedulerLocator
    {
        public static ITurnScheduler Service { get; set; }
    }
}
