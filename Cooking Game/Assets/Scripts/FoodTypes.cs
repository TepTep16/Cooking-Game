using UnityEngine;

public class FoodTypes : MonoBehaviour
{
    [SerializeField] private FoodType foodType;
    public enum FoodType
    {
        Vegetable,
        Fruit,
        Meat,
        Dairy,
        Grain
    }
}
