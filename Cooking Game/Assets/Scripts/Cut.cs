using UnityEngine;

public class RaycastDeactivate : MonoBehaviour
{
    public float maxDistance = 50f;

    void Update()
    {
        // Define the ray starting from this object moving forward
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            // Optional: Check for a specific tag so you don't accidently disable the floor/walls
            if (hit.collider.CompareTag("Targetable"))
            {
                // Deactivate the specific GameObject that was hit
                hit.collider.gameObject.SetActive(false);
            }
        }
    }
}


