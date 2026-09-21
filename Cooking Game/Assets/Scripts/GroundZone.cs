using UnityEngine;
using System.Collections.Generic;

public class GroundZone : MonoBehaviour
{
    [Tooltip("Drag the GameObject with the AnxietyManager component onto this slot.")]
    public AnxietyManager anxietyManager;

    // This part is to prevent mutiple counts on the anxiety meter if the object remains on the floor
    private HashSet<Collider> countedColliders = new HashSet<Collider>();

    private void OnCollisionEnter(Collision collision)
    {
        Collider other = collision.collider;
        if (countedColliders.Contains(other)) return;

        bool isFood = other.GetComponent<Ingredient>() != null || other.GetComponent<FoodItem>() != null;
        if (!isFood) return;

        countedColliders.Add(other);

        if (anxietyManager != null)
        {
            anxietyManager.OnFoodDropped();
        }
    }
}
