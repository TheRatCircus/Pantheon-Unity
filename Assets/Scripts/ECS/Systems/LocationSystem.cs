// LocationSystem.cs
// Jerome Martina

using Pantheon.ECS.Components;
using Pantheon.UI;
using Pantheon.Utils;
using Pantheon.World;
using System.Diagnostics;

namespace Pantheon.ECS.Systems
{
    public sealed class LocationSystem : ComponentSystem
    {
        public LocationSystem(EntityManager mgr, GameLog log)
            : base(mgr, log) { }

        public override void Update()
        {
            Stopwatch watch = Stopwatch.StartNew();
            int i = 0;
            foreach (Location loc in mgr.Locations)
            {
                if (loc == null)
                    continue;

                if (i == mgr.End)
                    break;

                if (!loc.Moving)
                    continue;

                Entity e = mgr.Entities[loc.GUID];

                if (Cell.Walkable(loc.Cmd.cell))
                {
                    if (loc.Cell != null)
                        loc.Cell.Actor = null;

                    loc.Cell = loc.Cmd.cell;
                    loc.Cell.Actor = e;
                    loc.Level = loc.Cmd.level;
                    e.GameObjects[0].transform.position = loc.Cell.Position.ToVector3();
                }
                else { } // Do nothing

                loc.ResetCommand();

                i++;
            }
            watch.Stop();
            UnityEngine.Debug.Log($"LocationSystem update took {watch.ElapsedMilliseconds} ms.");
        }
    }
}
