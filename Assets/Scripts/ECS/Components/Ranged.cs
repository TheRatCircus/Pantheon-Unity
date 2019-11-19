// Ranged.cs
// Jerome Martina

namespace Pantheon.ECS.Components
{   
    /// <summary>
    /// Allows an entity to be used as a ranged weapon.
    /// </summary>
    [System.Serializable]
    public sealed class Ranged : BaseComponent
    {
        [UnityEngine.SerializeField] private Attack[] attacks;

        public override BaseComponent Clone()
        {
            throw new System.NotImplementedException();
        }
    }
}
