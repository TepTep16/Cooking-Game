using UnityEngine;

public class Audiencebar : MonoBehaviour
{
    public float speed = 5f; // Speed of the beam
    public Transform PointA; // Starting point of the beam
    public Transform PointB;
    public Transform PointC;
    private Vector3 targetPosition; // Target position for the beam to move towards
    void Start()
    {
        targetPosition = PointB.position;
        targetPosition = PointC.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Move towards the current target
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Check if the object has successfully reached the destination
        if (transform.position == targetPosition)
        {
            // If it just reached Point B, change the target to Point C
            if (targetPosition == PointB.position)
            {
                targetPosition = PointC.position;
            }
           
        }
    }

}

