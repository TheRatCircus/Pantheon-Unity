// HealthBar.cs
// Jerome Martina

using UnityEngine;
using UnityEngine.UI;
using Pantheon.Actors;
using Pantheon.World;
using Pantheon.Utils;

namespace Pantheon.UI
{
    /// <summary>
    /// Handles health bar overlays.
    /// </summary>
    public class OnscreenHealthBar : MonoBehaviour
    {
        // Requisite objects
        [SerializeField] private GameObject prefab = null;

        // UI elements
        Transform ui;
        Image healthSlider;

        // Start is called before the first frame update
        void Start()
        {
            foreach (Canvas c in FindObjectsOfType<Canvas>())
                if (c.renderMode == RenderMode.WorldSpace)
                {
                    ui = Instantiate(prefab, c.transform).transform;
                    healthSlider = ui.GetChild(0).GetComponent<Image>();
                    ui.gameObject.SetActive(false);
                    break;
                }

            Actor targetActor = GetComponent<Actor>();
            targetActor.OnHealthChangeEvent += OnHealthChange;
            targetActor.OnMoveEvent += OnMove;
        }

        // Handle OnHealthChange event
        void OnHealthChange(int health, int maxHealth)
        {
            if (ui != null)
            {
                ui.gameObject.SetActive(true);
                healthSlider.fillAmount = (float)health / maxHealth;
                if (health <= 0)
                    Destroy(ui.gameObject);
            }
        }

        // Handle OnMove event
        private void OnMove(Cell cell)
        {
            Reposition(cell);
        }

        // Reposition this health bar to a new cell
        private void Reposition(Cell cell)
        {
            Vector3 newPosition = Helpers.V2IToV3(cell.Position);
            newPosition.y -= .35f;
            ui.position = newPosition;
        }
    }
}
