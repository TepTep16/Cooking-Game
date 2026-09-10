using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oven : MonoBehaviour
{
    [Header("Recipe")]
    [Tooltip("The ingredientIDs this oven accepts, exactly as typed into each Ingredient's 'Ingredient ID' field. Duplicates are allowed (e.g. \"Onion\", \"Onion\", \"Meat\" needs two onions and a meat).")]
    public List<string> requiredIngredientIDs = new List<string> { "Tomato", "Onion", "Meat" };

    [Tooltip("If true, an ingredient must be fully chopped before the oven will accept it.")]
    public bool requireFullyChopped = true;

    [Tooltip("If true, an ingredient still parented to the player's hold point (i.e. still being carried) will be ignored, forcing the player to drop/throw it in instead of just walking up to the oven while holding it.")]
    public bool ignoreWhileHeld = true;

    [Header("Bounce Back (incorrect ingredient)")]
    public float bounceForce = 6f;
    public float bounceUpwardBoost = 0.3f;

   

    [Tooltip("Optional. Leave this empty to just spawn the finished food directly above the oven — that's the default behavior.")]
    public Transform foodSpawnPoint;
    public GameObject completedFoodPrefab;

    [Tooltip("Delay before the food spawns, so it can line up with the door-opening animation.")]
    public float spawnDelay = 0.5f;

    private List<string> remainingIngredientIDs;
    private bool isComplete = false;

    void Awake()
    {
        ResetOven();
    }

    // Call this externally if you want to reuse the oven for another round.
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

        // In case it's still flagged as held/kinematic, let physics take over so it can actually bounce.
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

    private IEnumerator SpawnFoodAfterDelay()
    {
        yield return new WaitForSeconds(spawnDelay);

        if (completedFoodPrefab != null)
        {
            Vector3 spawnPos = foodSpawnPoint != null ? foodSpawnPoint.position : transform.position + Vector3.up;
            Quaternion spawnRot = foodSpawnPoint != null ? foodSpawnPoint.rotation : transform.rotation;
            Instantiate(completedFoodPrefab, spawnPos, spawnRot);
        }
    }
}
