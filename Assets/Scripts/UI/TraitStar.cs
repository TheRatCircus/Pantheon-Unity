// TraitStar.cs
// Jerome Martina

using UnityEngine;
using UnityEngine.UI;

namespace Pantheon.UI
{
    public class TraitStar : MonoBehaviour
    {
        [SerializeField] private GameObject connectionPrefab = null;

        [SerializeField] private Button button = null;
        [SerializeField] private TraitRef traitRef = TraitRef.None;
        [SerializeField] private TraitStar[] prereqs = null;
        public bool Acquired { get; private set; }
        public System.Action AcquiredEvent;

        public TraitRef TraitRef { get => traitRef; }
        public TraitStar[] Prereqs { get => prereqs; }

        private void Start()
        {
            if (prereqs.Length > 0)
                button.interactable = false;

            foreach (TraitStar p in prereqs)
            {
                // Connect to prereqs by lines
                RectTransform otherTransform = p.GetComponent<RectTransform>();
                GameObject newConnectionObj = Instantiate(
                    connectionPrefab, transform.position,
                    new Quaternion(), transform);
                LineRenderer lineRenderer
                    = newConnectionObj.GetComponent<LineRenderer>();

                Vector3 delta = otherTransform.localPosition
                    - transform.localPosition;
                lineRenderer.SetPosition(1, delta);

                p.AcquiredEvent += RecomputePrereqs;
            }
        }

        public void Acquire()
        {
            button.targetGraphic.color = Color.yellow;
            Acquired = true;
            AcquiredEvent?.Invoke();
        }

        public void RecomputePrereqs()
        {
            bool prereqsMet = true;
            foreach (TraitStar ts in prereqs)
                if (!ts.Acquired)
                    prereqsMet = false;

            if (prereqsMet)
                button.interactable = true;
        }
    }
}
