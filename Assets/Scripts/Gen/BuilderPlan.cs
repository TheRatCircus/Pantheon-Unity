// BuilderPlan.cs
// Jerome Martina

using Pantheon.Utils;

namespace Pantheon.Gen
{
    /// <summary>
    /// An invocable set of instructions for procedurally generating levels.
    /// </summary>
    public sealed class BuilderPlan
    {
        public string ID { get; set; } = "DEFAULT_PLAN_ID";
        public BuilderStep[] Steps { get; set; }
        public GenericRandomPick<string>[] Population { get; set; }

        public override string ToString() => ID;
    }
}
