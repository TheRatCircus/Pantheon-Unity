// HUD.cs
// Jerome Martina

using Pantheon.Core;
using Pantheon.World;
using UnityEngine;

namespace Pantheon.UI
{
    public sealed class HUD : MonoBehaviour
    {
        [SerializeField] private GameObject modalListPrefab = default;

        [SerializeField] private HUDHealth health = default;
        [SerializeField] private HUDEnergy energy = default;
        [SerializeField] private HUDClock clock = default;
        [SerializeField] private HUDLocation location = default;

        private GameObject modalList;

        public void Initialize(TurnScheduler scheduler, Entity player,
            Level activeLevel, System.Action<Level> levelChangeEvent)
        {
            health.Initialize(player);
            energy.Initialize(player);
            clock.Initialize(scheduler);
            location.Initialize(activeLevel, levelChangeEvent);
        }

        public ModalList OpenModalList()
        {
            GameObject modalListObj = Instantiate(modalListPrefab, transform);
            modalList = modalListObj;
            return modalListObj.GetComponent<ModalList>();
        }

        public void ClearModalList()
        {
            Destroy(modalList);
            modalList = null;
        }
    }
}
