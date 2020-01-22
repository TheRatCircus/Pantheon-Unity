// Talent.cs
// Jerome Martina

using Pantheon.Commands;
using Pantheon.World;

namespace Pantheon
{
    [System.Serializable]
    public abstract class TalentComponent
    {
        public abstract CommandResult Invoke(Entity caster, Cell target);
    }
}