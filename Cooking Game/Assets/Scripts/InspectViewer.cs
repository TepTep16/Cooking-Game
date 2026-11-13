using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class InspectViewer : MonoBehaviour
{
    
    public Camera playerCamera;
    public float interactRange = 5f;
   
    public LayerMask interactableLayer;

    [Header("UI")]
    
    public GameObject viewerPanel;
    
    public Image displayImage;

    [Header("Audio")]
   
    public AudioSource voiceSource;
    
    public bool stopVoiceOnClose = true;

    [Tooltip("e.g. PickUp, mouse look, RecipeBook, HoverHighlight, AdviceDisplay")]
    public Behaviour[] disableWhileOpen;

    private bool isOpen = false;
    private float previousTimeScale = 1f;

    void Awake()
    {
        if (voiceSource == null)
        {
            voiceSource = gameObject.AddComponent<AudioSource>();
            voiceSource.playOnAwake = false;
            voiceSource.spatialBlend = 0f; 
        }
    }

    void Start()
    {
        if (viewerPanel != null)
            viewerPanel.SetActive(false);
    }

    void Update()
    {
        if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame) return;

        if (isOpen)
        {
            Close();
            return;
        }

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer))
        {
            InspectableObject inspectable = hit.collider.GetComponentInParent<InspectableObject>();
            if (inspectable != null)
                Open(inspectable);
        }
    }

    private void Open(InspectableObject inspectable)
    {
        isOpen = true;

        if (displayImage != null)
        {
            displayImage.sprite = inspectable.image;
            displayImage.preserveAspect = true;
        }

        if (viewerPanel != null)
            viewerPanel.SetActive(true);

        if (inspectable.voiceline != null)
        {
            voiceSource.Stop();
            voiceSource.PlayOneShot(inspectable.voiceline);
        }

        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        foreach (Behaviour script in disableWhileOpen)
        {
            if (script != null)
                script.enabled = false;
        }
    }

    private void Close()
    {
        isOpen = false;

        if (viewerPanel != null)
            viewerPanel.SetActive(false);

        if (stopVoiceOnClose)
            voiceSource.Stop();

        Time.timeScale = previousTimeScale;

        StartCoroutine(ReenableScriptsNextFrame());
    }

    
    private IEnumerator ReenableScriptsNextFrame()
    {
        yield return null;

        foreach (Behaviour script in disableWhileOpen)
        {
            if (script != null)
                script.enabled = true;
        }
    }
}
