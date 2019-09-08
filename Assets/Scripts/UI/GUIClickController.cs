// Custom event script handling different mouse clicks
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Pantheon.UI
{
    public class GUIClickController : MonoBehaviour, IPointerClickHandler
    {
        public UnityEvent onLeft;
        public UnityEvent onRight;
        public UnityEvent onMiddle;

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
