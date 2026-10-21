using UnityEngine;
using TMPro;


public class NameplateUI : MonoBehaviour
{
   
    public float range = 5f;

    public LayerMask interactableLayer;

    public GameObject nameplatePanel;

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
