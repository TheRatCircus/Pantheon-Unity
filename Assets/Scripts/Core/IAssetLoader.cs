// IAssetLoader.cs
// Jerome Martina

using Pantheon.Content;
using UnityEngine;

namespace Pantheon.Core
{
    public interface IAssetLoader
    {
        T Load<T>(string name) where T : Object;
        EntityTemplate LoadTemplate(string name);
        SpeciesDefinition LoadSpeciesDef(string name);
        BodyPart LoadBodyPart(string name);
        bool AssetExists(string name);
    }
}
