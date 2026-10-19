using UnityEngine;
using UnityEngine.InputSystem;

public class PickUp : MonoBehaviour
{
    //This script mainly manages the pickup/drop (LMB) and chopping (RMB) mechanics for Ingredients, plus a few extra features that got added along the way.
    public GameObject player;
    public Transform holdPos;
    public float pickUpRange = 5f;
    //REMEMBER THIS
    public float dropDistance = 1.5f;
    //THIS PART IS FOR WALL BLOCKING, TO AVOID CLIPPING
    public float dropWallPadding = 0.2f;
    //Layer we're using for the ingredients.
    public LayerMask interactableLayer;
    public PlayerInputHandler playerInputHandler;
    private Ingredient heldIngredient;
    private bool canDrop = true;
    private int LayerNumber;
    public float throwAmount;

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

    //REMEMBER THIS
    void DropObject()
    {
        Vector3 dropPosition = GetDropPosition();

        RestorePhysicalState();
        heldIngredient.Drop(dropPosition, heldIngredient.transform.rotation, null);
        heldIngredient = null;
    }

    //REMEMBER THIS (FOR WALL BLOCKING, TO AVOID CLIPPING)
    private Vector3 GetDropPosition()
    {
        // 0 = old behavior
        if (dropDistance <= 0f) return heldIngredient.transform.position;
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;
        //remeber this
        int mask = ~(1 << LayerNumber);
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, dropDistance, mask, QueryTriggerInteraction.Ignore))
        {
            return hit.point - direction * dropWallPadding;
        }

        return origin + direction * dropDistance;
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
