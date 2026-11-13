using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyChef : MonoBehaviour
{
    // One entry per meal this chef cooks (e.g. Garlic Bread, Fish & Chips).
    // The chef finishes them in the order they appear in the list.
    [System.Serializable]
    public class Meal
    {
        public string mealName;            // just a label for you in the Inspector
        public Oven oven;                  // the oven that cooks this meal
        public Transform servingStation;   // where the finished meal gets dished up
    }

    private enum State
    {
        FindIngredient, GoToIngredient, Chopping, GoToOven, WaitForOven,
        WaitForFood, GoToFood, GoToStation, Done
    }

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float sprintDistance = 10f;   // sprint if destination is further than this
    [SerializeField] private float interactDistance = 2f;  // how close counts as "arrived"

    [SerializeField] private float ovenInteractDistance = 0.8f;  // how close to get before dropping

    [Header("Chopping")]
    [SerializeField] private float chopInterval = 0.5f;    // seconds between chops
    [SerializeField] private float dropWaitTime = 2f;      // seconds to wait for the oven to take the ingredient

    [Header("Dishing Up")]
    [SerializeField] private float foodPickupDistance = 1.5f;     // how close to get to the cooked meal to grab it
    [SerializeField] private float stationInteractDistance = 1.2f; // how close to get to the station before dropping
    [SerializeField] private float stationDropHeight = 0.2f;       // drop the meal slightly above the station so it lands on it

    [Header("References")]
    [SerializeField] private Meal[] meals;                 // the meals this chef makes (oven + serving station each)
    [SerializeField] private Transform holdPoint;          // where carried ingredients attach

    private NavMeshAgent agent;
    private State state = State.FindIngredient;
    private int mealIndex = 0;

    private Ingredient target;    // ingredient we're heading to / chopping
    private Ingredient carried;   // ingredient we're holding
    private float chopTimer;

    private Ingredient dropped;   // the ingredient we just dropped
    private float waitTimer;
    private float debugTimer;     // For Debugging

    private GameObject mealObject; // the cooked meal we are fetching / carrying

    // The oven of the meal we're currently working on
    private Oven myOven => meals[mealIndex].oven;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        bool ok = holdPoint != null && meals != null && meals.Length > 0;

        if (ok)
        {
            foreach (Meal m in meals)
            {
                if (m.oven == null || m.servingStation == null) { ok = false; break; }
            }
        }

        if (!ok)
        {
            Debug.LogError("EnemyChef: assign holdPoint and at least one Meal (with an oven AND a serving station) in the Inspector.");
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
            case State.WaitForOven: WaitForOven(); break;
            case State.WaitForFood: WaitForFood(); break;
            case State.GoToFood: GoToFood(); break;
            case State.GoToStation: GoToStation(); break;
            case State.Done: break;
        }
    }

    // 1. Pick the closest loose ingredient that my oven still needs
    private void FindIngredient()
    {
        if (myOven.IsComplete)
        {
            // All ingredients are in, now go and collect the cooked meal
            agent.ResetPath();
            state = State.WaitForFood;
            return;
        }

        Ingredient best = null;
        float bestDist = float.MaxValue;

        foreach (Ingredient ing in FindObjectsByType<Ingredient>(FindObjectsSortMode.None))
        {
            if (ing.transform.parent != null) continue;                         // held by someone
            if (!myOven.RemainingIngredientIDs.Contains(ing.ingredientID)) continue; // not needed

            if (ing == dropped) continue;   // don't re-grab what we just dropped

            float d = Vector3.Distance(transform.position, ing.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = ing;
            }
        }

        if (best == null)           // For Debugging
        {
            debugTimer += Time.deltaTime;
            if (debugTimer >= 2f)
            {
                debugTimer = 0f;
                Debug.Log("Chef waiting. Oven still needs: " + string.Join(", ", myOven.RemainingIngredientIDs));
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

        if (IsNear(target.transform.position, interactDistance))
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
        Vector3 ovenPos = myOven.transform.position;
        MoveTo(ovenPos);

        // The oven may sit on a counter that isn't walkable, so the chef can't always
        // reach its center. Also count it as arrived when the agent reaches the end of its path.
        bool reachedPathEnd = !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f;

        if (IsNear(ovenPos, ovenInteractDistance) || reachedPathEnd)
        {
            agent.ResetPath();

            // Drop at the oven's trigger, not at its pivot
            Vector3 dropPos = myOven.transform.position + Vector3.up;
            foreach (Collider c in myOven.GetComponentsInChildren<Collider>())
            {
                if (c.isTrigger) { dropPos = c.bounds.center; break; }
            }

            dropped = carried;
            carried.Drop(dropPos, Quaternion.identity);
            carried = null;
            waitTimer = 0f;
            state = State.WaitForOven;
        }
    }

    private void WaitForOven()
    {
        waitTimer += Time.deltaTime;

        if (dropped == null || waitTimer >= dropWaitTime)
        {
            state = State.FindIngredient;
        }
    }

    // 5. The oven has all its ingredients: wait for the cooked meal to appear
    private void WaitForFood()
    {
        if (!myOven.FoodWasSpawned) return;   // oven still cooking (spawn delay)

        mealObject = myOven.SpawnedFood;

        if (mealObject == null)
        {
            // Oven has no meal prefab, or the meal was destroyed: move on
            FinishMeal();
            return;
        }

        state = State.GoToFood;
    }

    // 6. Walk to the cooked meal and pick it up
    private void GoToFood()
    {
        // Meal was destroyed or someone else (the player) took it: move on
        if (mealObject == null || mealObject.transform.parent != null)
        {
            mealObject = null;
            FinishMeal();
            return;
        }

        Vector3 foodPos = mealObject.transform.position;
        MoveTo(foodPos);

        bool reachedPathEnd = !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f;

        if (IsNear(foodPos, foodPickupDistance) || reachedPathEnd)
        {
            agent.ResetPath();
            PickUpMeal(mealObject);
            state = State.GoToStation;
        }
    }

    // 7. Carry the meal to this meal's serving station and dish it up
    private void GoToStation()
    {
        Vector3 stationPos = meals[mealIndex].servingStation.position;
        MoveTo(stationPos);

        // Station may be on a counter that isn't walkable, so also accept "end of path"
        bool reachedPathEnd = !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f;

        if (IsNear(stationPos, stationInteractDistance) || reachedPathEnd)
        {
            agent.ResetPath();
            DropMeal(mealObject, stationPos + Vector3.up * stationDropHeight);
            Debug.Log("Chef dished up: " + meals[mealIndex].mealName);   // For Debugging
            mealObject = null;
            FinishMeal();
        }
    }

    // Move on to the next meal, or stop if all meals are dished up
    private void FinishMeal()
    {
        mealIndex++;
        target = null;
        carried = null;
        dropped = null;

        if (mealIndex >= meals.Length)
        {
            mealIndex = meals.Length - 1;   // keep the property valid
            agent.ResetPath();
            Debug.Log("All meals dished up, chef is done.");   // For Debugging
            state = State.Done;
        }
        else
        {
            state = State.FindIngredient;
        }
    }

    private void PickUpMeal(GameObject meal)
    {
        Ingredient ing = meal.GetComponent<Ingredient>();
        if (ing != null)
        {
            ing.PickUp(holdPoint);
            return;
        }

        // Fallback for meal prefabs without an Ingredient script
        Rigidbody rb = meal.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        meal.transform.SetParent(holdPoint);
        meal.transform.localPosition = Vector3.zero;
        meal.transform.localRotation = Quaternion.identity;
    }

    private void DropMeal(GameObject meal, Vector3 position)
    {
        Ingredient ing = meal.GetComponent<Ingredient>();
        if (ing != null)
        {
            ing.Drop(position, Quaternion.identity);
            return;
        }

        meal.transform.SetParent(null);
        meal.transform.position = position;
        meal.transform.rotation = Quaternion.identity;

        Rigidbody rb = meal.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }

    private void MoveTo(Vector3 destination)
    {
        float dist = Vector3.Distance(transform.position, destination);
        agent.speed = dist > sprintDistance ? sprintSpeed : walkSpeed;
        agent.SetDestination(destination);
    }

    // Ignores height so a tall oven/ingredient doesn't break the check
    private bool IsNear(Vector3 point, float distance)
    {
        Vector3 a = transform.position; a.y = 0f;
        Vector3 b = point; b.y = 0f;
        return Vector3.Distance(a, b) <= distance;
    }
}
