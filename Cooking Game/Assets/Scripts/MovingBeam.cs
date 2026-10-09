using UnityEngine;
using System.Collections;   
using System.Collections.Generic;

public class MovingBeam : MonoBehaviour
{
    public float speed = 5f; // Speed of the beam
    public Transform PointA; // Starting point of the beam
    public Transform PointB;
    private Vector3 targetPosition; // Target position for the beam to move towards
    void Start()
    {
        targetPosition = PointB.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        
    }
}
