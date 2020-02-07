// IEntityBasedTalent.cs
// Jerome Martina

namespace Pantheon.Components.Talent
{
    using Entity = Pantheon.Entity;

    public interface IEntityBasedTalent
    {
        Entity Entity { get; set; }
    }
}
