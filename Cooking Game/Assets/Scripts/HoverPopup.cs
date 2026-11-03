using UnityEngine;
using UnityEngine.EventSystems; 

public class HoverPopup : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject popupObject; 

    void Start()
    {
        // Ensure the popup is hidden at the start
        if (popupObject != null)
        {
            popupObject.SetActive(false);
        }
    }

    // Triggers when the mouse hovers over the button
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (popupObject != null)
        {
            popupObject.SetActive(true);
        }
    }

    // Triggers when the mouse leaves the button
    public void OnPointerExit(PointerEventData eventData)
    {
        if (popupObject != null)
        {
            popupObject.SetActive(false);
        }
    }
}
