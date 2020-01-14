using Pantheon.World;
using UnityEngine;

namespace Pantheon.Core
{
    public sealed class Game : MonoBehaviour
    {
        private GameWorld world;

        private void Start()
        {
            world = new GameWorld();
        }
    }
}
