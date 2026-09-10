using UnityEngine;

// Put this on the finished food prefab (the thing the Oven spawns).
public class FoodItem : MonoBehaviour
{
    [Tooltip("Identifies what type of finished food this is (e.g. \"Soup\", \"Salad\"). Used by systems like the Platform to check if it's the correct item.")]
    public string foodID;
}
