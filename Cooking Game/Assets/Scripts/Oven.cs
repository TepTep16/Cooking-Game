using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oven : MonoBehaviour
{
    [Header("Recipe")]
    public List<string> requiredIngredientIDs = new List<string> { "Tomato", "Onion", "Meat" };
    public bool requireFullyChopped = true;
    public bool ignoreWhileHeld = true;

    [Header("Bounce Back (incorrect ingredient)")]
    public float bounceForce = 6f;
    public float bounceUpwardBoost = 0.3f;

    //For Particle effect after cooking
    [Header("Completion Effect")]
    [Tooltip("Particle effect prefab instantiated on the cooked food when it spawns. Automatically destroyed after 5 seconds.")]
    public GameObject completionParticlePrefab;


    public Transform foodSpawnPoint;
    public GameObject completedFoodPrefab;

   //have to use this part for the visual smoke effect, or a door animation at some point
    public float spawnDelay = 0.5f;

    private List<string> remainingIngredientIDs;
    private bool isComplete = false;

    void Awake()
    {
        ResetOven();
    }

    // This part must be called externally if the round resets
    public void ResetOven()
    {
        remainingIngredientIDs = new List<string>(requiredIngredientIDs);
        isComplete = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isComplete) return;

        Ingredient ingredient = other.GetComponent<Ingredient>();
        if (ingredient == null) return;

        if (ignoreWhileHeld && ingredient.transform.parent != null) return;

        if (IsCorrectIngredient(ingredient))
        {
            AcceptIngredient(ingredient);
        }
        else
        {
            BounceBack(ingredient, other);
        }
    }

    private bool IsCorrectIngredient(Ingredient ingredient)
    {
        if (requireFullyChopped && !ingredient.isFullyChopped) return false;
        return remainingIngredientIDs.Contains(ingredient.ingredientID);
    }

    private void AcceptIngredient(Ingredient ingredient)
    {
        // Remove only one matching entry, so recipes needing 2x of something work correctly.
        remainingIngredientIDs.Remove(ingredient.ingredientID);
        Destroy(ingredient.gameObject);

        if (remainingIngredientIDs.Count <= 0)
        {
            CompleteRecipe();
        }
    }

    private void BounceBack(Ingredient ingredient, Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb == null) return;

        // This part allows it to bounce back even if it was kinematic (e.g. being held by the player)
        rb.isKinematic = false;
        rb.useGravity = true;

        Vector3 pushDirection = (other.transform.position - transform.position);
        pushDirection.y = 0f;
        pushDirection = pushDirection.sqrMagnitude > 0.001f ? pushDirection.normalized : -transform.forward;
        pushDirection += Vector3.up * bounceUpwardBoost;

        rb.linearVelocity = Vector3.zero;
        rb.AddForce(pushDirection * bounceForce, ForceMode.Impulse);
    }

    private void CompleteRecipe()
    {
        isComplete = true;
        StartCoroutine(SpawnFoodAfterDelay());
    }

    //Modified the SpawnFoodAfterDelay function to integrate smoking effect after cooking is finished.
    private IEnumerator SpawnFoodAfterDelay()
    {
        yield return new WaitForSeconds(spawnDelay);

        if (completedFoodPrefab != null)
        {
            Vector3 spawnPos = foodSpawnPoint != null ? foodSpawnPoint.position : transform.position + Vector3.up;
            Quaternion spawnRot = foodSpawnPoint != null ? foodSpawnPoint.rotation : transform.rotation;

            GameObject spawnedFood = Instantiate(completedFoodPrefab, spawnPos, spawnRot);

            if (completionParticlePrefab != null)
            {
                // Parented to spawnedFood.transform so the effect follows the food if it moves (e.g. gets picked up).
                GameObject particleInstance = Instantiate(completionParticlePrefab, spawnedFood.transform.position, spawnedFood.transform.rotation, spawnedFood.transform);

                // Destroy(gameObject, delay) schedules removal after 5 seconds without needing a separate coroutine or script.
                Destroy(particleInstance, 5f);
            }
        }
    }
}
