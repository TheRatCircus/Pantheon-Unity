// LoaderLocator.cs
// Jerome Martina

using Pantheon.Core;
using Pantheon.UI;

namespace Pantheon
{
    public static class Locator
    {
        public static IGameLog Log { get; set; }
        //public static IAssetLoader Loader { get; set; }
        public static ITurnScheduler Scheduler { get; set; }
        = new NullTurnScheduler();
        public static IPlayer Player { get; set; }
    }
}
