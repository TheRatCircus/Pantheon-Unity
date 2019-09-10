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
    public class OnScreenHealthBar : MonoBehaviour
    {
        // Requisite objects
        [SerializeField] private GameObject prefab = null;

        // UI elements
        private Actor actor;
        private Transform ui;
        private Image healthSlider;

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

            actor = GetComponent<Actor>();
            actor.OnHealthChangeEvent += OnHealthChange;
            actor.OnMoveEvent += OnMove;

            if (actor is NPC)
                ((NPC)actor).OnVisibilityChangeEvent += SetVisibility;

            Reposition(actor.Cell);
        }

        // Handle OnHealthChange event
        private void OnHealthChange(int health, int maxHealth)
        {
            if (ui != null)
            {
                if (health >= maxHealth)
                    ui.gameObject.SetActive(false);
                else
                    ui.gameObject.SetActive(true);

                healthSlider.fillAmount = (float)health / maxHealth;

                if (health <= 0)
                    Destroy(ui.gameObject);
            }
        }

        // Handle OnMove event
        private void OnMove(Cell cell) => Reposition(cell);

        // Reposition this health bar to a new cell
        private void Reposition(Cell cell)
        {
            Vector3 newPosition = Helpers.V2IToV3(cell.Position);
            newPosition.y -= .35f;
            ui.position = newPosition;
        }

        private void SetVisibility(bool visible)
        {
            if (actor.Health < actor.MaxHealth)
                ui.gameObject.SetActive(visible);
        }
    }
}
