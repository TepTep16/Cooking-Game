using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class NewHoverScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float scaleFactor = 1.2f; // The factor by which to enlarge the object
    private Vector3 originalScale; // The original scale of the object
    private Transform RectTransform; // The Transform component of the object
    
    private void Awake()
    {
        RectTransform = GetComponent<Transform>();
        originalScale = RectTransform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        RectTransform.localScale = originalScale * scaleFactor;
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        RectTransform.localScale = originalScale;
        
    }
}