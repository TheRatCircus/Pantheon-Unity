// LogLocator.cs
// Jerome Martina

namespace Pantheon.UI
{
    public static class LogLocator
    {
        public static IGameLog _log { get; private set; }

        public static void Provide(IGameLog log)
        {
            _log = log;
        }
    }
}