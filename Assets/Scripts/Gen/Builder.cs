// Builder.cs
// Jerome Martina

using Pantheon.World;

namespace Pantheon.Gen
{
    /// <summary>
    /// An object used to generate a level upon request.
    /// </summary>
    public sealed class Builder
    {
        private string displayName;
        private string id;
        private BuilderPlan plan;

        public Builder(string displayName, string id, BuilderPlan plan)
        {
            this.displayName = displayName;
            this.id = id;
            this.plan = plan;
        }

        public void Run(Level level)
        {
            level.DisplayName = displayName;
            level.ID = id;
            plan.Run(level, 200, 200);
            level.RebuildPathfinder();
        }
    }
}
