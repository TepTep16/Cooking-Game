using UnityEngine;
using  System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class HoverEnlarger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
   public float scaleFactor = 1.2f; // The factor by which to enlarge the object
   private Vector3 originalScale; // The original scale of the object
   private Transform RectTransform; // The Transform component of the object
    [SerializeField] private GameObject popupObject;

    private void Awake()
    {
        RectTransform = GetComponent<Transform>();
        originalScale = RectTransform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        RectTransform.localScale = originalScale * scaleFactor;
        if (popupObject != null)
        {
            popupObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        RectTransform.localScale = originalScale;
        if (popupObject != null)
        {
            popupObject.SetActive(false);
        }
    }
}