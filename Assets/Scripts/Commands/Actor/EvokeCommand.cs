// EvokeCommand.cs
// Jerome Martina

using Pantheon.Components;
using Pantheon.Utils;
using System;
using UnityEngine;

namespace Pantheon.Commands.Actor
{
    public sealed class EvokeCommand : ActorCommand
    {
        private readonly Entity item;

        [NonSerialized] private Vector2Int cell;
        [NonSerialized] private Line line;
        [NonSerialized] private Line path;
        public Vector2Int Cell { get => cell; set => cell = value; }
        public Line Line { get => line; set => line = value; }
        public Line Path { get => path; set => path = value; }

        public EvokeCommand(Entity entity, Entity evocable) : base(entity)
        {
            item = evocable;
        }

        public override CommandResult Execute(out int cost)
        {
            if (!item.TryGetComponent(out Evocable evoc))
            {
                cost = -1;
                return CommandResult.Failed;
            }
            else
            {
                CommandResult result;
                if (Cell != null && Line != null && Path != null)
                    result = evoc.Evoke(Entity, 0, Cell, Line, Path);
                else
                    result = evoc.Evoke(Entity, 0);

                if (result != CommandResult.Succeeded)
                    cost = -1;
                else
                    cost = evoc.EvokeTime;

                return result;
            }
        }
    }
}
