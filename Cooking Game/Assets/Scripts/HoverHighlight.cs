using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Raycasts from screen center every frame. While the player looks directly at an
/// Ingredient or any object with a Hoverable component, a UI panel appears on screen
/// with its name and a hint, and hides again when the player looks away.
/// This script never modifies the objects it looks at.
/// </summary>
public class HoverHighlight : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public float hoverRange = 5f;
    [Tooltip("Must include the layers of ingredients and every Hoverable object")]
    public LayerMask interactableLayer;

    [Header("Ingredient Defaults")]
    public string ingredientHint = "LMB: pick up   RMB: chop";

    [Header("UI")]
    [Tooltip("UI panel shown while looking at something - start it inactive in the scene")]
    public GameObject tooltipPanel;
    [Tooltip("Text inside the panel - swap to TMP_Text if you're using TextMeshPro")]
    public TMP_Text tooltipText;

    [Tooltip("If true, the panel follows the object on screen. If false, it stays where you placed it on the canvas.")]
    public bool followObject = false;
    public Vector2 screenOffset = new Vector2(20f, 20f);

    private Transform currentTarget;

    void Update()
    {
        Transform target = FindTarget(out string label, out string hint);

        if (target != currentTarget)
        {
            currentTarget = target;

            if (tooltipPanel != null)
                tooltipPanel.SetActive(target != null);

            if (target != null && tooltipText != null)
                tooltipText.text = string.IsNullOrEmpty(hint) ? label : label + "\n" + hint;
        }

        if (followObject && currentTarget != null && tooltipPanel != null)
        {
            Vector3 screenPos = playerCamera.WorldToScreenPoint(currentTarget.position);
            tooltipPanel.transform.position = screenPos + (Vector3)screenOffset;
        }
    }

    private Transform FindTarget(out string label, out string hint)
    {
        label = null;
        hint = null;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (!Physics.Raycast(ray, out RaycastHit hit, hoverRange, interactableLayer))
            return null;

        // Any object with a Hoverable component (collider may be on a child)
        Hoverable hoverable = hit.collider.GetComponentInParent<Hoverable>();
        if (hoverable != null)
        {
            label = hoverable.displayName;
            hint = hoverable.hint;
            return hoverable.transform;
        }

        // Ingredients work automatically, no extra component needed
        Ingredient ingredient = hit.collider.GetComponent<Ingredient>();
        if (ingredient != null)
        {
            label = ingredient.ingredientID;
            hint = ingredientHint;
            return ingredient.transform;
        }

        return null;
    }
}