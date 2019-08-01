// Handles on-map health bars
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public GameObject prefab;
    public Transform target;

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

    private void OnMove(Cell cell)
    {
        Reposition(cell);
    }

    private void Reposition(Cell cell)
    {
        Vector3 newPosition = Helpers.V2IToV3(cell.Position);
        newPosition.y -= .2f;
        ui.position = newPosition;
    }
}
