// ConsoleCommandFunctions.cs
// Jerome Martina

using Pantheon.Core;
using Pantheon.ECS.Components;
using Pantheon.Utils;
using Pantheon.World;
using System;
using System.Collections.Generic;

namespace Pantheon.Debug
{
    public static class ConsoleCommandFunctions
    {
        public static string ListLayers(string[] args, GameController ctrl)
        {
            string ret = "";
            foreach (Layer layer in ctrl.World.Layers.Values)
                ret += $"layer {layer.ZLevel} {Environment.NewLine}";
            return ret;
        }

        public static string ListLevels(string[] args, GameController ctrl)
        {
            string ret = "";
            foreach (Layer layer in ctrl.World.Layers.Values)
                foreach (Level level in layer.Levels.Values)
                    ret += $"{level.ID}";
            return ret;
        }

        public static string WhereAmI(string[] args, GameController ctrl)
        {
            Position playerPos = ctrl.Manager.Player.GetComponent<Position>();
            return playerPos.ToString();
        }
    }
}
