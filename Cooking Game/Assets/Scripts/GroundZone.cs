using UnityEngine;
using System.Collections.Generic;

public class GroundZone : MonoBehaviour
{
    public AnxietyManager anxietyManager;

    // This is to stop items on the floor from double triggering collisions
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
