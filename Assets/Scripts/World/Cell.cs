// Cell.cs
// Jerome Martina

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Pantheon.Components.Entity;
using Pantheon.Content;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.World
{
    [Flags]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CellFlag : byte
    {
        Visible = 1 << 0,
        Revealed = 1 << 1
    }
}
