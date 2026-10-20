using UnityEngine;
using System.Collections.Generic;

// Put this on the platform the player places finished food onto.
// Requires a (non-trigger) Collider + Rigidbody setup so OnCollisionEnter actually fires.
public class Platform : MonoBehaviour
{
    [Header("Recipe")]
    [Tooltip("Which foodIDs count as correct for this platform. List the same ID multiple times if you need multiples of one type (e.g. \"Soup\", \"Soup\", \"Salad\").")]
    public List<string> acceptedFoodIDs = new List<string> { "Soup", "Soup", "Soup" };

    [Tooltip("How many correct food items need to be placed before success triggers. Usually matches the size of Accepted Food IDs above.")]
    public int requiredCount = 3;

    [Header("UI")]
    [Tooltip("The success screen GameObject (e.g. a Canvas or Panel) to activate once enough correct food has been placed. Leave empty if the GameTimer is handling the win screen instead.")]
    public GameObject successScreen;

    private int correctCount = 0;
    private bool isComplete = false;

    // Read by GameTimer to check whether the player finished in time.
    public bool IsComplete => isComplete;
    public int CorrectCount => correctCount;

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