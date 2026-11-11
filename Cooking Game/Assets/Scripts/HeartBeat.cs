using UnityEngine;

public class HeartBeat : MonoBehaviour
{
    public float beatInterval = 1.0f; // Time between beats in seconds
    public float beatScale = 1.2f; // Scale factor for the heartbeat effect
    public float beatDuration = 0.2f; // Duration of the heartbeat effect in seconds
    void Start()
    {
        InvokeRepeating("Beat", 0f, beatInterval);
    }

    void Beat()
    {
        StartCoroutine(DoBeat());
    }

    System.Collections.IEnumerator DoBeat()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * beatScale;

        float timer = 0f;
        while (timer < beatDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, timer / beatDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localScale = originalScale;
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
