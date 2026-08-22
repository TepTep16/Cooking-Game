using UnityEngine;
using UnityEngine.InputSystem;

public class PickUp : MonoBehaviour
{
    //This script handles pickup/drop (LMB) and chopping (RMB) for Ingredients,
    //plus a couple of extra features (layer swap, collision ignore, clip prevention).
    //Platform placement is NOT handled here - ChoppingPlatform auto-snaps anything
    //dropped into its trigger zone, so this script doesn't need to know it exists.

    [Header("References")]
    public GameObject player;
    public Transform holdPos;

    [Header("Settings")]
    public float pickUpRange = 5f;

    [Tooltip("Set this to a layer that includes ingredients")]
    public LayerMask interactableLayer;

    private Ingredient heldIngredient;
    private bool canDrop = true;
    private int LayerNumber;

    void Start()
    {
        LayerNumber = LayerMask.NameToLayer("StableLayer");
    }

    void Update()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (heldIngredient == null)
            {
                TryPickUp();
            }
            else if (canDrop)
            {
                TryDrop();
            }
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            TryChop();
        }
    }

    private bool TryFindInteractable(out Collider result)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange, interactableLayer))
        {
            result = hit.collider;
            return true;
        }

        result = null;
        return false;
    }

    private void TryPickUp()
    {
        if (!TryFindInteractable(out Collider hitCollider)) return;

        Ingredient ingredient = hitCollider.GetComponent<Ingredient>();
        if (ingredient == null) return;

        PickUpObject(ingredient);
    }

    private void TryDrop()
    {
        // Just drop it into the world with physics
        StopClipping();
        DropObject();
    }

    private void TryChop()
    {
        if (!TryFindInteractable(out Collider hitCollider)) return;

        Ingredient ingredient = hitCollider.GetComponent<Ingredient>();

        // Chop anything you're looking at, as long as it's not the ingredient
        // currently in your hand (holdPos sits directly in the raycast's path)
        if (ingredient != null && ingredient != heldIngredient)
        {
            ingredient.Chop();
        }
    }

    //here is where the ingredient is picked up and held, it also sets the layer of the object to a stable layer, so that it doesn't collide with the player
    void PickUpObject(Ingredient ingredient)
    {
        heldIngredient = ingredient;
        ingredient.PickUp(holdPos);
        ingredient.gameObject.layer = LayerNumber;
        Physics.IgnoreCollision(ingredient.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
    }

    void DropObject()
    {
        RestorePhysicalState();
        heldIngredient.Drop(heldIngredient.transform.position, heldIngredient.transform.rotation, null);
        heldIngredient = null;
    }

    private void RestorePhysicalState()
    {
        Physics.IgnoreCollision(heldIngredient.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldIngredient.gameObject.layer = 0;
    }

    //might remove this part of the script if it doesnt work, found it off of youtube
    void StopClipping()
    {
        var clipRange = Vector3.Distance(heldIngredient.transform.position, player.transform.position);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(player.transform.position, player.transform.TransformDirection(Vector3.forward), clipRange);
        if (hits.Length > 1)
        {
            heldIngredient.transform.position = transform.position + new Vector3(0f, -0.5f, 0f);
        }
    }
}
