using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WiggleEffect : MonoBehaviour

{
    [Header("Horizontal Wiggle")]
    public float frequencyX = 3f;
    public float amplitudeX = 0.3f;

    [Header("Vertical Wiggle")]
    public float frequencyY = 2.5f;
    public float amplitudeY = 0.3f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.localPosition;
    }

    void Update()
    {
        float x = Mathf.Sin(Time.time * frequencyX) * amplitudeX;
        float y = Mathf.Sin(Time.time * frequencyY) * amplitudeY;

        transform.localPosition = startPosition + new Vector3(x, y, 0f);
    }
}
