using UnityEngine;

// Put on an empty GameObject with a Box Collider (Is Trigger ticked).
// When the player walks into it, the tutorial moves on (if it is waiting for this eventKey).
[RequireComponent(typeof(Collider))]
public class TutorialTrigger : MonoBehaviour
{
    [Tooltip("NearCheese, NearCuttingBoard or NearOven")]
    [SerializeField] private string eventKey = "NearCheese";

    void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerStay(Collider other)
    {
        if (TutorialManager.Instance == null) return;
        if (other.GetComponent<CharacterController>() == null) return; // only the player
        if (TutorialManager.Instance.IsWaitingFor(eventKey))
            TutorialManager.Instance.Notify(eventKey);
    }
}
