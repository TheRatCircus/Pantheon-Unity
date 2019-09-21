// FlaskData.cs
// Jerome Martina

using UnityEngine;

/// <summary>
/// Template for a flask.
/// </summary>
[CreateAssetMenu(fileName = "New Flask", menuName = "BaseData/Items/Flask")]
public class FlaskData : ItemData
{
    [SerializeField] private FlaskType flaskType = FlaskType.None;

    public FlaskType FlaskType { get => flaskType; }
}
