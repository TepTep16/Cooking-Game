using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpriteSpawner : MonoBehaviour
{
    public GameObject sprite; // The sprite prefab to spawn
    void Start()
    {
        StartCoroutine(SpawnSprites());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public IEnumerator SpawnSprites()
    {
        while (true)
        {
            GameObject newsprite = Instantiate(sprite);
            newsprite.transform.position = new Vector3(Random.value * -8-1, Random.value * 5-3, 0f);
            Destroy(newsprite, 1.5f); // Destroy the sprite after 5 seconds
            // Wait for 1 second before spawning the next sprite
            yield return new WaitForSeconds(1f);
        }
    }
}
