using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyChef : MonoBehaviour
{
    private enum State { FindIngredient, GoToIngredient, Chopping, GoToOven, Done }

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float sprintDistance = 10f;   // sprint if destination is further than this
    [SerializeField] private float interactDistance = 2f;  // how close counts as "arrived"

    [Header("Chopping")]
    [SerializeField] private float chopInterval = 0.5f;    // seconds between chops

    [Header("References")]
    [SerializeField] private Oven myOven;                  // this chef's OWN oven
    [SerializeField] private Transform holdPoint;          // where carried ingredients attach

    private NavMeshAgent agent;
    private State state = State.FindIngredient;
    private Ingredient target;    // ingredient we're heading to / chopping
    private Ingredient carried;   // ingredient we're holding
    private float chopTimer;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        if (myOven == null || holdPoint == null)
        {
            Debug.LogError("EnemyChef: assign myOven and holdPoint in the Inspector.");
            enabled = false;
        }
    }

    void Update()
    {
        switch (state)
        {
            case State.FindIngredient: FindIngredient(); break;
            case State.GoToIngredient: GoToIngredient(); break;
            case State.Chopping: Chop(); break;
            case State.GoToOven: GoToOven(); break;
            case State.Done: break;
        }
    }

    // 1. Pick the closest loose ingredient that my oven still needs
    private void FindIngredient()
    {
        if (myOven.IsComplete)
        {
            agent.ResetPath();
            state = State.Done;
            return;
        }

        Ingredient best = null;
        float bestDist = float.MaxValue;

        foreach (Ingredient ing in FindObjectsByType<Ingredient>(FindObjectsSortMode.None))
        {
            if (ing.transform.parent != null) continue;                         // held by someone
            if (!myOven.RemainingIngredientIDs.Contains(ing.ingredientID)) continue; // not needed

            float d = Vector3.Distance(transform.position, ing.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = ing;
            }
        }

        if (best != null)
        {
            target = best;
            state = State.GoToIngredient;
        }
    }

    // 2. Walk (or sprint) to it
    private void GoToIngredient()
    {
        // Someone (e.g. the player) picked it up or it was destroyed: pick a new one
        if (target == null || target.transform.parent != null)
        {
            state = State.FindIngredient;
            return;
        }

        MoveTo(target.transform.position);

        if (IsNear(target.transform.position))
        {
            agent.ResetPath();
            chopTimer = 0f;
            state = State.Chopping;
        }
    }

    // 3. Chop until fully chopped, then pick it up
    private void Chop()
    {
        if (target == null || target.transform.parent != null)
        {
            state = State.FindIngredient;
            return;
        }

        if (target.isFullyChopped)
        {
            carried = target;
            carried.PickUp(holdPoint);
            target = null;
            state = State.GoToOven;
            return;
        }

        chopTimer += Time.deltaTime;
        if (chopTimer >= chopInterval)
        {
            chopTimer = 0f;
            target.Chop();
        }
    }

    // 4. Carry it to my oven and drop it in
    private void GoToOven()
    {
        MoveTo(myOven.transform.position);

        if (IsNear(myOven.transform.position))
        {
            agent.ResetPath();
            carried.Drop(myOven.transform.position + Vector3.up, Quaternion.identity);
            carried = null;
            state = State.FindIngredient;   // repeat
        }
    }

    private void MoveTo(Vector3 destination)
    {
        float dist = Vector3.Distance(transform.position, destination);
        agent.speed = dist > sprintDistance ? sprintSpeed : walkSpeed;
        agent.SetDestination(destination);
    }

    // Ignores height so a tall oven/ingredient doesn't break the check
    private bool IsNear(Vector3 point)
    {
        Vector3 a = transform.position; a.y = 0f;
        Vector3 b = point; b.y = 0f;
        return Vector3.Distance(a, b) <= interactDistance;
    }
}