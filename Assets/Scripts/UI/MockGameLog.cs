// MockGameLog.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon.UI
{
    public sealed class MockGameLog : IGameLog
    {
        public void Send(string msg, Color color) { }
    }
}
