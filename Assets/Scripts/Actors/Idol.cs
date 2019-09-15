// Idol.cs
// Jerome Martina

namespace Pantheon.Actors
{
    /// <summary>
    /// Represents abstract information about an Idol.
    /// </summary>
    public class Idol
    {
        public string DisplayName { get; set; }
        public string RefName { get; set; }

        public Idol(string displayName, string refName)
        {
            DisplayName = displayName;
            RefName = refName;
        }
    }
}
