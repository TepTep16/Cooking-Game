using UnityEngine;

/// <summary>
/// Attach to any ingredient prefab. Handles being picked up/dropped,
/// and swapping visual stages as it gets chopped.
/// </summary>
[RequireComponent(typeof(Collider))]
public class Ingredient : MonoBehaviour
{
    [Header("Chop Stages")]
    [Tooltip("Visual stages of this ingredient, in order. Element 0 = whole/uncut, last element = fully chopped. " +
             "These should be child GameObjects (different meshes) that this script enables/disables one at a time.")]
    public GameObject[] chopStages;

    [Tooltip("How many right-click hits are needed to advance to the next stage")]
    public int hitsPerStage = 5;

    [Header("State (read-only, for debugging)")]
    public bool isFullyChopped = false;

    private int currentStageIndex = 0;
    private int currentHitCount = 0;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        SetStageVisuals(0);
    }

    /// <summary>Called by the player when picking this ingredient up.</summary>
    public void PickUp(Transform holdPoint)
    {
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    /// <summary>Called by the player when dropping. Drops freely into the world with physics.</summary>
    public void Drop(Vector3 worldPosition, Quaternion worldRotation, Transform newParent = null)
    {
        transform.SetParent(newParent);
        transform.position = worldPosition;
        transform.rotation = worldRotation;

        if (rb != null)
        {
            rb.isKinematic = newParent != null;
            rb.useGravity = newParent == null;
        }
    }

    /// <summary>Registers one chop hit. Returns true once fully chopped.</summary>
    public bool Chop()
    {
        if (isFullyChopped) return false;

        currentHitCount++;

        if (currentHitCount >= hitsPerStage)
        {
            currentHitCount = 0;
            currentStageIndex++;

            if (currentStageIndex >= chopStages.Length - 1)
            {
                currentStageIndex = chopStages.Length - 1;
                isFullyChopped = true;
            }

            SetStageVisuals(currentStageIndex);
        }

        return isFullyChopped;
    }

    private void SetStageVisuals(int index)
    {
        for (int i = 0; i < chopStages.Length; i++)
        {
            if (chopStages[i] != null)
                chopStages[i].SetActive(i == index);
        }
    }
}
