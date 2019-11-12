// Blocking.cs
// Jerome Martina

namespace Pantheon.ECS.Components
{
    [System.Serializable]
    public sealed class Blocking : BaseComponent
    {
        public bool IsBlocking = true; // Switch on/off for doors, etc.

        public override BaseComponent Clone()
        {
            return new Blocking();
        }
    }
}
