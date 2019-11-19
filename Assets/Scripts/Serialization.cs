// Serialization.cs
// Jerome Martina

using Newtonsoft.Json.Serialization;
using Pantheon.ECS.Components;
using Pantheon.Gen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pantheon
{
    public static class Serialization
    {
        public static readonly SerializationBinder _builderStepBinder
            = new SerializationBinder
            {
                KnownTypes = new List<Type>()
                {
                    typeof(Fill),
                    typeof(RandomFill)
                }
            };

        public static readonly SerializationBinder _entityBinder
            = new SerializationBinder
            {
                KnownTypes = new List<Type>()
                {
                    typeof(Health),
                    typeof(Melee),
                    typeof(Actor)
                }
            };
    }

    public sealed class SerializationBinder : ISerializationBinder
    {
        public IList<Type> KnownTypes { get; set; }

        public void BindToName(Type serializedType, out string assemblyName,
            out string typeName)
        {
            assemblyName = null;
            typeName = serializedType.Name;
        }

        public Type BindToType(string assemblyName, string typeName)
        {
            return KnownTypes.SingleOrDefault(t => t.Name == typeName);
        }
    }
}
