using UnityEngine;

public class EnemyChef : MonoBehaviour
{
    private Rigidbody2D myBody;

    private float walkSpeed = 4.0f;
    private float sprintSpeed = 8.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
    }
}
