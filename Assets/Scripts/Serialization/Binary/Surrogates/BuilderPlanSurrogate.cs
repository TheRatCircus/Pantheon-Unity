// BuilderPlanSurrogate.cs
// Jerome Martina

using Pantheon.Core;
using Pantheon.Gen;
using System.Runtime.Serialization;

namespace Pantheon.Serialization.Binary.Surrogates
{
    public sealed class BuilderPlanSurrogate : ISerializationSurrogate
    {
        private AssetLoader loader;

        public BuilderPlanSurrogate(AssetLoader loader)
        {
            this.loader = loader;
        }

        public void GetObjectData(object obj, SerializationInfo info,
            StreamingContext context)
        {
            BuilderPlan plan = (BuilderPlan)obj;
            info.AddValue("id", plan.ID);
        }

        public object SetObjectData(object obj, SerializationInfo info,
            StreamingContext context, ISurrogateSelector selector)
        {
            BuilderPlan plan = (BuilderPlan)obj;
            plan = loader.LoadPlan(info.GetString("id"));
            obj = plan;
            return obj;
        }
    }
}
