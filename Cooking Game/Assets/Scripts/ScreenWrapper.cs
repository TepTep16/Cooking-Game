using UnityEngine;

public class ScreenWrapper : MonoBehaviour
{
    private Camera mainCamera;
    private float screenWidthInWorld;
    private float screenHeightInWorld;

    void Start()
    {
        mainCamera = Camera.main;

        // Calculate the screen dimensions in Unity world units
        // Vector3(Screen.width, Screen.height, distance from camera)
        float cameraZ = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
        Vector3 screenTopRightWorld = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, cameraZ));

        // Boundaries from center (0,0) to the edges
        screenWidthInWorld = screenTopRightWorld.x;
        screenHeightInWorld = screenTopRightWorld.y;
    }

    void LateUpdate()
    {
        Vector3 currentPosition = transform.position;

        // Horizontal wrapping (Left <-> Right)
        if (currentPosition.x > screenWidthInWorld)
        {
            currentPosition.x = -screenWidthInWorld;
        }
        else if (currentPosition.x < -screenWidthInWorld)
        {
            currentPosition.x = screenWidthInWorld;
        }

        // Vertical wrapping (Top <-> Bottom) - Optional
        if (currentPosition.y > screenHeightInWorld)
        {
            currentPosition.y = -screenHeightInWorld;
        }
        else if (currentPosition.y < -screenHeightInWorld)
        {
            currentPosition.y = screenHeightInWorld;
        }

        transform.position = currentPosition;
    }
}

