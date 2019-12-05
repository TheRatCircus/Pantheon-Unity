// Actor.cs
// Jerome Martina

using Pantheon.Commands;
using Pantheon.Utils;
using Pantheon.World;
using UnityEngine;

namespace Pantheon
{
    public sealed class Actor : MonoBehaviour
    {
        public Level Level { get; set; }
        public Cell Cell { get; set; }

        public ActorCommand Command { get; set; }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Move(Level level, Cell cell)
        {
            Level = level;
            Cell = cell;
            transform.position = cell.Position.ToVector3();
        }

        public bool HostileTo(Actor other) => true;
    }
}
