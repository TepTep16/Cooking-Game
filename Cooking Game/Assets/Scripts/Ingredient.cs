using UnityEngine;


[RequireComponent(typeof(Collider))]
public class Ingredient : MonoBehaviour
{
    [Header("Chop Stages")]
    [Tooltip("The changes in the ingredient game object as it's being chopped by the player. Element 0 = whole/uncut, Element 1 = fully chopped. ")]
    public GameObject[] chopStages;

    [Tooltip("How many rmb clicks are needed to break down the ingredient")]
    public int hitsPerStage = 5;

    [Header("(read-only, for debugging)")]
    public bool isFullyChopped = false;

    private int currentStageIndex = 0;
    private int currentHitCount = 0;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        SetStageVisuals(0);
    }
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
