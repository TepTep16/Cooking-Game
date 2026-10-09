using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageFader : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float fadeDelay = 0f;
    [SerializeField] private float visibleDuration = 1f; // How long the image stays visible before fading
    [SerializeField] private Image imageA;
    [SerializeField] private Image imageB;
    
    
    void Start()
    {
        // initial state: Image A visible, Image B hidden, canvas fully visible
        imageA.gameObject.SetActive(true);
        imageB.gameObject.SetActive(false);
        canvasGroup.alpha = 1f;

        StartCoroutine(FadeImages());
    }
    private IEnumerator FadeImages()
    {
        yield return new WaitForSeconds(fadeDelay);

        // Keep track of which image is currently showing
        bool showingA = true;

        while (true)
        {
            // 1. Wait while the current image is fully visible
            yield return new WaitForSeconds(visibleDuration);

            // 2. Fade Out (Alpha 1 to 0)
            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            canvasGroup.alpha = 0f;

            // 3. Swap the active images
            if (showingA)
            {
                imageA.gameObject.SetActive(false);
                imageB.gameObject.SetActive(true);
            }
            else
            {
                imageB.gameObject.SetActive(false);
                imageA.gameObject.SetActive(true);
            }

            // Flip the state tracking 
            showingA = !showingA;

            //  Fade In (Alpha 0 to 1)
            elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            canvasGroup.alpha = 1f;
        }
    }
}

