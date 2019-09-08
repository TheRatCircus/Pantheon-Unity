// Custom event script handling different mouse clicks
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Pantheon.UI
{
    public class GUIClickController : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private UnityEvent onLeft = null;
        [SerializeField] private UnityEvent onRight = null;
        [SerializeField] private UnityEvent onMiddle = null;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.pointerId == -1)
                onLeft.Invoke();
            else if (eventData.pointerId == -2)
                onRight.Invoke();
            else if (eventData.pointerId == -3)
                onMiddle.Invoke();
        }
    }
}
