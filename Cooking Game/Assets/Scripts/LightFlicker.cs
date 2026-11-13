using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    private Light myLight;
    public float minIntensity = 0.5f;
    public float maxIntensity = 1.5f;
    public float flickerSPEED = 0.1f;

    private void Start()
    {
        myLight = GetComponent<Light>();
           InvokeRepeating("Flicker", 0f, flickerSPEED);
    }

    private void Flicker()
    {
        float randomIntensity = Random.Range(minIntensity, maxIntensity);
        myLight.intensity = randomIntensity;
    }
}