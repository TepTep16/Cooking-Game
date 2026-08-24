using UnityEngine;
using UnityEngine.InputSystem;

public class PickUp : MonoBehaviour
{
    //This script mainly manages the pickup/drop (LMB) and chopping (RMB) mechanics for Ingredients, plus a few extra features that got added along the way.
    public GameObject player;
    public Transform holdPos;
    public float pickUpRange = 5f;

    [Tooltip("Layer used for ingredients")]
    public LayerMask interactableLayer;

    [Tooltip("Reference to the PlayerInputHandler holding the Pickup/Chop actions")]
    public PlayerInputHandler playerInputHandler;

    private Ingredient heldIngredient;
    private bool canDrop = true;
    private int LayerNumber;

    void Start()
    {
        LayerNumber = LayerMask.NameToLayer("StableLayer");
    }

    void Update()
    {
        if (playerInputHandler.PickupAction.WasPressedThisFrame())
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

        if (playerInputHandler.ChopAction.WasPressedThisFrame())
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
        StopClipping();
        DropObject();
    }

    private void TryChop()
    {
        if (!TryFindInteractable(out Collider hitCollider)) return;

        Ingredient ingredient = hitCollider.GetComponent<Ingredient>();
        // this was done to allow the player to cut any ingredient that they're looking at, as long as its not currently in hand.
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
        heldIngredient.gameObject.layer = 6;
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
