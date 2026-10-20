using UnityEngine;
using TMPro;

// Put this on the same object as your PickUp script (the player/camera).
// Every frame, casts a ray forward and shows a nameplate UI element with the
// name of whatever Ingredient or FoodItem is currently being aimed at.
public class NameplateUI : MonoBehaviour
{
    [Header("Raycast Settings")]
    [Tooltip("How far the raycast checks for a nameable object. Match this to PickUp's pickUpRange if you want the nameplate to only show things you could actually interact with.")]
    public float range = 5f;

    [Tooltip("Layer used for ingredients/food - same layer your PickUp script uses.")]
    public LayerMask interactableLayer;

    [Header("UI References")]
    [Tooltip("The UI element (e.g. a Panel holding a TMP text) that shows/hides as a whole. Should start inactive.")]
    public GameObject nameplatePanel;

    [Tooltip("The TextMeshPro text that displays the object's name.")]
    public TMP_Text nameplateText;

    void Update()
    {
        RaycastHit hit;
        bool didHit = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, range, interactableLayer);

        if (!didHit)
        {
            HideNameplate();
            return;
        }

        string displayName = GetDisplayName(hit.collider);

        if (string.IsNullOrEmpty(displayName))
        {
            HideNameplate();
            return;
        }

        ShowNameplate(displayName);
    }

    private string GetDisplayName(Collider hitCollider)
    {
        Ingredient ingredient = hitCollider.GetComponent<Ingredient>();
        if (ingredient != null)
        {
            return ingredient.ingredientID;
        }

        FoodItem food = hitCollider.GetComponent<FoodItem>();
        if (food != null)
        {
            return food.foodID;
        }

        return null;
    }

    private void ShowNameplate(string displayName)
    {
        if (nameplatePanel != null) nameplatePanel.SetActive(true);
        if (nameplateText != null) nameplateText.text = displayName;
    }

    private void HideNameplate()
    {
        if (nameplatePanel != null) nameplatePanel.SetActive(false);
    }
}
