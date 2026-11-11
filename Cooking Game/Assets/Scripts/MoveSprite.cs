using UnityEngine;

public class MoveSprite : MonoBehaviour

{
    public float speed = 5f;
    public float distance = 3f;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Sine wave creates a smooth continuous left-and-right loop
        float xOffset = Mathf.Sin(Time.time * speed) * distance;
        transform.position = new Vector3(startPosition.x + xOffset, startPosition.y, startPosition.z);
    }
}

