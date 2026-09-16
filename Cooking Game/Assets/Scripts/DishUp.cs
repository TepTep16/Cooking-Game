using System.Collections.Generic;
using UnityEngine;

public class DishUp : MonoBehaviour
{
    [Header("Recipe")]
    [Tooltip("Which foods (foodIDs) are needed for the recipe")]
    public List<string> acceptedFoodIDs = new List<string> { "Sandwich" };

    [Tooltip("How many food items need to be placed on the platform to count as a win.")]
    public int requiredCount = 3;

    [Header("UI")]
    [Tooltip("UI that's activate afterall food items have been placed.")]
    public GameObject winScreen;

    private int correctCount = 0;
    private bool isComplete = false;

    // This part of the script tracks the colliders that have already been counted, so an object resting on the platform wont get counted twice (due to repeated collision events).
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

        if (winScreen != null)
        {
            winScreen.SetActive(true);
        }
    }

    // This part should be called externally if there's more than 1 round of the game.
    public void ResetPlatform()
    {
        correctCount = 0;
        isComplete = false;
        countedColliders.Clear();

        if (winScreen != null)
        {
            winScreen.SetActive(false);
        }
    }
}


