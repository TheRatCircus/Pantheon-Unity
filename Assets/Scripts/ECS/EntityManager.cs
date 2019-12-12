// EntityManager.cs
// Jerome Martina

using Pantheon.ECS.Components;
using Pantheon.World;

namespace Pantheon.ECS
{
    [System.Serializable]
    public sealed class EntityManager
    {
        private const int MaxEntities = 200;

        public int End { get; private set; } = 0; // Current end of array
        private int currentEntity = 0;

        public Entity[] Entities { get; private set; }
        public Location[] Locations { get; private set; }
        public Actor[] Actors { get; private set; }
        public AI[] AI { get; private set; }

        private Entity player;
        public Entity Player
        {
            get => player;
            set { player = value; PlayerActor = Actors[value.GUID]; }
        }
        public Actor PlayerActor { get; private set; }

        public event System.Action<Actor, Level, bool> NewActorEvent;

        public EntityManager()
        {
            Entities = new Entity[MaxEntities];
            Locations = new Location[MaxEntities];
            Actors = new Actor[MaxEntities];
            AI = new AI[MaxEntities];
        }

        public Entity NewEntity(EntityTemplate template,
            Level level, Cell cell)
        {
            Entity entity = new Entity(template);
            entity.GUID = currentEntity;
            Entities[currentEntity] = entity;

            Locations[currentEntity] = new Location();
            Location loc = Locations[currentEntity];
            loc.GUID = currentEntity;
            loc.Level = level;
            loc.Cell = cell;

            if (template.TryGetComponent(out Actor actor))
            {
                Actor newActor = (Actor)actor.Clone();
                newActor.GUID = currentEntity;
                Actors[currentEntity] = newActor;
                NewActorEvent?.Invoke(newActor, level, cell.Visible);
                cell.Actor = entity;
            }
            if (template.TryGetComponent(out AI ai))
            {
                AI newAI = (AI)ai.Clone();
                newAI.GUID = currentEntity;
                AI[currentEntity] = newAI;
            }

            End++;
            currentEntity++;
            return entity;
        }

        public Cell CellOf(Entity entity) => Locations[entity.GUID].Cell;
        public Cell CellOf(EntityComponent ec) => Locations[ec.GUID].Cell;
        public Level LevelOf(Entity entity) => Locations[entity.GUID].Level;

        public Cell CellOfPlayer() => Locations[Player.GUID].Cell;
        public Level LevelOfPlayer() => Locations[Player.GUID].Level;
        public Location PlayerLocation() => Locations[Player.GUID];
    }
}
