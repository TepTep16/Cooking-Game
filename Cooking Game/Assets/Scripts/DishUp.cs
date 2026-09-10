using System.Collections.Generic;
using UnityEngine;

public class DishUp : MonoBehaviour
{
    [Header("Recipe")]
    [Tooltip("Which foodIDs count as correct for this platform. List the same ID multiple times if you need multiples of one type (e.g. \"Soup\", \"Soup\", \"Salad\").")]
    public List<string> acceptedFoodIDs = new List<string> { "Grilled Cheese", "Soup", "Juice" };

    [Tooltip("How many correct food items need to be placed before success triggers. Usually matches the size of Accepted Food IDs above.")]
    public int requiredCount = 3;

    [Header("UI")]
    [Tooltip("The success screen GameObject (e.g. a Canvas or Panel) to activate once enough correct food has been placed.")]
    public GameObject successScreen;

    private int correctCount = 0;
    private bool isComplete = false;

    // Tracks which colliders have already been counted, so an object resting on the
    // platform and generating repeated collision events doesn't get counted twice.
    private HashSet<Collider> countedColliders = new HashSet<Collider>();

    private void OnCollisionEnter(Collision collision)
    {
        if (isComplete) return;

        Collider other = collision.collider;
        if (countedColliders.Contains(other)) return;

        FoodItem food = other.GetComponent<FoodItem>();
        if (food == null) return;

        if (!acceptedFoodIDs.Contains(food.foodID)) return;

        countedColliders.Add(other);
        correctCount++;

        if (correctCount >= requiredCount)
        {
            TriggerSuccess();
        }
    }

    private void TriggerSuccess()
    {
        isComplete = true;

        if (successScreen != null)
        {
            successScreen.SetActive(true);
        }
    }

    // Call this externally if you want to reuse the platform for another round.
    public void ResetPlatform()
    {
        correctCount = 0;
        isComplete = false;
        countedColliders.Clear();

        if (successScreen != null)
        {
            successScreen.SetActive(false);
        }
    }
}


