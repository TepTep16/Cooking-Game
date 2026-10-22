using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class HoverHighlight : MonoBehaviour
{
  
    public Camera playerCamera;
    public float hoverRange = 5f;
    public LayerMask interactableLayer;

    [Header("Default Hints for the ingredients")]
    public string ingredientHint = "LMB: pick up   RMB: chop";

    public GameObject tooltipPanel;
    public TMP_Text tooltipText;

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

       
        Hoverable hoverable = hit.collider.GetComponentInParent<Hoverable>();
        if (hoverable != null)
        {
            label = hoverable.displayName;
            hint = hoverable.hint;
            return hoverable.transform;
        }

        
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