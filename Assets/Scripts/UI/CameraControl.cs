// CameraControl.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon.Core
{
    /// <summary>
    /// Allows the game camera to be dragged around.
    /// </summary>
    public sealed class CameraControl : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetMouseButton(1))
            {
                if (Input.GetAxis("MouseX") != 0 ||
                    Input.GetAxis("MouseY") != 0)
                {
                    transform.localPosition += 
                        new Vector3(
                            Input.GetAxis("MouseX") * .1f,
                            Input.GetAxis("MouseY") * .1f,
                            -1);
                }
            }
            else
            {
                if (transform.localPosition != Vector3.zero)
                    transform.localPosition = new Vector3(0, 0, -1);
            }
        }
    }
}
