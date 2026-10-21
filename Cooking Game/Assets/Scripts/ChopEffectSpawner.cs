using UnityEngine;

public class ChopEffectSpawner : MonoBehaviour
{
    [SerializeField] private ParticleSystem chopEffect;
    [SerializeField] private int particlesPerChop = 10;

    void OnEnable()  { Ingredient.Chopped += OnChopped; }
    void OnDisable() { Ingredient.Chopped -= OnChopped; }

    private void OnChopped(Ingredient ingredient)
    {
        if (chopEffect == null) return;

        Vector3 center = ingredient.transform.position;
        float size = 0.1f;

        Renderer[] rends = ingredient.GetComponentsInChildren<Renderer>();
        if (rends.Length > 0)
        {
            Bounds b = rends[0].bounds;
            for (int i = 1; i < rends.Length; i++) b.Encapsulate(rends[i].bounds);
            center = b.center;
            size = b.extents.magnitude;
        }

        chopEffect.transform.position = center;

        // bigger ingredient = slightly wider burst
        var shape = chopEffect.shape;
        shape.radius = Mathf.Clamp(size * 0.4f, 0.03f, 0.25f);

        chopEffect.Emit(particlesPerChop);
    }
}
