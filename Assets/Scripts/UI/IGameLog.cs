// IGameLog.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon.UI
{
    public interface IGameLog
    {
        void Send(string msg, Color color);
    }
}
