using System;
using UnityEngine;


[RequireComponent(typeof(Collider))]
public class Ingredient : MonoBehaviour
{
    // Events the tutorial listens to
    public static event Action<Ingredient> PickedUp;
    public static event Action<Ingredient> Dropped;
    public static event Action<Ingredient> FullyChopped;
    // fires on EVERY successful chop hit (used by the cut-line particle effect)
    public static event Action<Ingredient> Chopped;

    [Header("Identification")]
    [Tooltip("Used by systems like the Oven to identify what this ingredient is (e.g. a \"Tomato\", \"Onion\")")]

    public string ingredientID;
    public GameObject[] chopStages;

    [Tooltip("Untick for things that can never be cut (milk, milkshake, cooked meals...)")]
    public bool canBeChopped = true;

    
    public AudioClip[] chopSounds;

   
    public AudioClip fullyChoppedSound;

    [Range(0f, 1f)] public float chopVolume = 1f;

    
    [Range(0f, 0.5f)] public float pitchVariation = 0.1f;

    public int hitsPerStage = 5;


    //FOR DEBUGGING
    public bool isFullyChopped = false;
    private int currentStageIndex = 0;
    private int currentHitCount = 0;
    private Rigidbody rb;
    private AudioSource audioSource;
    public bool CanChop => canBeChopped && !isFullyChopped && chopStages != null && chopStages.Length > 1;
    public int CurrentStage => currentStageIndex;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f; 
        }
        audioSource.playOnAwake = false;
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
        PickedUp?.Invoke(this);
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
        Dropped?.Invoke(this);
    }
    public bool Chop()
    {
        if (!canBeChopped) return false;
        if (isFullyChopped) return false;

        currentHitCount++;
        Chopped?.Invoke(this);

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
            if (isFullyChopped) FullyChopped?.Invoke(this);
        }
        PlayChopSound(isFullyChopped);
        return isFullyChopped;
    }
    private void PlayChopSound(bool justFinished)
    {
        AudioClip clip = null;

        if (justFinished && fullyChoppedSound != null)
            clip = fullyChoppedSound;
        else if (chopSounds != null && chopSounds.Length > 0)
            clip = chopSounds[UnityEngine.Random.Range(0, chopSounds.Length)];

        if (clip == null || audioSource == null) return;

        audioSource.pitch = 1f + UnityEngine.Random.Range(-pitchVariation, pitchVariation);
        audioSource.PlayOneShot(clip, chopVolume);
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
