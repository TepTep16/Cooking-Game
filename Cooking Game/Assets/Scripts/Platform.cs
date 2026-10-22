using UnityEngine;
using System.Collections.Generic;


public class Platform : MonoBehaviour
{
    
    public List<string> acceptedFoodIDs = new List<string> { "Soup", "Soup", "Soup" };

    
    public int requiredCount = 3;


    public GameObject successScreen;

    private int correctCount = 0;
    private bool isComplete = false;

    // GameTimer reads this part to check if the player finished in time.
    public bool IsComplete => isComplete;
    public int CorrectCount => correctCount;


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