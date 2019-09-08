// Base class for flasks
using UnityEngine;
using Pantheon.Actions;

[CreateAssetMenu(fileName = "New Flask", menuName = "Items/Flask")]
public class FlaskData : ItemData
{
    public FlaskType _flaskType;
}
