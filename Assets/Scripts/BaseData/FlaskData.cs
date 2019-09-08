// Base class for flasks
using UnityEngine;

[CreateAssetMenu(fileName = "New Flask", menuName = "Items/Flask")]
public class FlaskData : ItemData
{
    [SerializeField] private FlaskType flaskType = FlaskType.None;

    public FlaskType FlaskType { get => flaskType; }
}
