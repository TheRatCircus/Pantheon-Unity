// Actor.cs
// Jerome Martina

using Pantheon.Commands;
using Pantheon.Utils;
using Pantheon.World;
using UnityEngine;

namespace Pantheon
{
    public enum ActorControl
    {
        None,
        AI,
        Player
    }

    public sealed class Actor : MonoBehaviour
    {
        public string Name { get; set; } = "DEFAULT_ACTOR_NAME";

        public Level Level { get; set; }
        public Cell Cell { get; set; }

        [SerializeField] private int speed = -1;
        public int Speed => speed;
        public int Energy { get; set; }
        public ActorCommand Command { get; set; }

        [SerializeField] public ActorControl control = default;
        public ActorControl Control
        {
            get => control;
            set => control = value;
        }

        private void Awake()
        {
            Energy = speed;
        }

        public int Act()
        {
            if (Command != null)
            {
                Command.Execute();
                return Command.Cost;
            }
            else
                return -1;
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
