using UnityEngine;

public class PickUp : MonoBehaviour
{
    //This script is for the pick up mechanic, which also includes a couple more features
    public GameObject player;
    public Transform holdPos;
    public float pickUpRange = 5f;
    private GameObject heldObj;
    private Rigidbody heldObjRb;
    private bool canDrop = true;
    private int LayerNumber;

    //this part is for the Mouse Look ScripT, which is used to rotate the object when it is being held

    void Start()
    {
        LayerNumber = LayerMask.NameToLayer("StableLayer");
        //mouseLookScript = player.GetComponent<MouseLook>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObj == null)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
                {
                    if (hit.transform.gameObject.tag == "canPickUp")
                    {
                        PickUpObject(hit.transform.gameObject);
                    }
                }
            }
            else
            {
                if (canDrop == true)
                {
                    StopClipping();
                    DropObject();
                }
            }
        }
        if (heldObj != null)
        {
            MoveObject();
        }
    }
    //here is where the object is picked up and held, it also sets the layer of the object to a stable layer, so that it doesn't collide with the player
    void PickUpObject(GameObject pickUpObj)
    {
        if (pickUpObj.GetComponent<Rigidbody>())
        {
            heldObj = pickUpObj;
            heldObjRb = pickUpObj.GetComponent<Rigidbody>();
            heldObjRb.isKinematic = true;
            heldObjRb.transform.parent = holdPos.transform;
            heldObj.layer = LayerNumber;
            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
        }
    }
    void MoveObject()
    {
        heldObj.transform.position = holdPos.transform.position;
    }
    void DropObject()
    {
        heldObjRb.isKinematic = false;
        heldObjRb.transform.parent = null;
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0;
        heldObj = null;
    }

    //might remove this part of the script if it doesnt work, found it off of youtube
    void StopClipping()
    {
        var clipRange = Vector3.Distance(heldObj.transform.position, player.transform.position);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(player.transform.position, player.transform.TransformDirection(Vector3.forward), clipRange);
        if (hits.Length > 1)
        {
            heldObj.transform.position = transform.position + new Vector3(0f, -0.5f, 0f);
        }
    }
}
