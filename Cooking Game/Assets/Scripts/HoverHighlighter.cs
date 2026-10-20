using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Looks at what the player's crosshair (screen centre) points at.
// Ingredient / FoodItem -> yellow overlay + "Pick up" and "cut" hints beside it.
public class HoverHighlighter : MonoBehaviour
{
    [Header("Scene references")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform holdPoint;          // the player's hold point (ignored by the ray)
    [SerializeField] private Canvas canvas;                // the GameUI canvas

    [Header("Highlight")]
    [SerializeField] private Material highlightMaterial;   // transparent yellow
    [SerializeField] private float reachDistance = 3f;     // match your pick-up range
    [SerializeField] private float overlayScale = 1.03f;

    [Header("Hint labels")]
    [SerializeField] private TMP_Text leftLabel;           // "Left click/LT Pick up"
    [SerializeField] private TMP_Text rightLabel;          // "Right click/RT x5 cut"
    [SerializeField] private float labelPadding = 20f;
    [SerializeField] private Color enabledColor = Color.white;
    [SerializeField] private Color disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.6f);

    private Transform current;
    private int lastStage = -1;
    private readonly List<GameObject> overlays = new List<GameObject>();

    void Start()
    {
        leftLabel.text = "Left click/LT Pick up";
        rightLabel.text = "Right click/RT x5 cut";
        leftLabel.alignment = TextAlignmentOptions.MidlineRight;
        rightLabel.alignment = TextAlignmentOptions.MidlineLeft;
        leftLabel.rectTransform.pivot = new Vector2(1f, 0.5f);
        rightLabel.rectTransform.pivot = new Vector2(0f, 0.5f);
        SetLabels(false);
    }

    void OnDisable()
    {
        ClearHighlight();
        if (leftLabel != null) SetLabels(false);
    }

    void Update()
    {
        Transform target = null;
        bool canChop = false;
        int stage = -1;

        bool blocked = (TutorialManager.DialogueOpen) || Time.timeScale == 0f;
        if (!blocked) FindTarget(out target, out canChop, out stage);

        if (target != current || (target != null && stage != lastStage))
        {
            ClearHighlight();
            current = target;
            lastStage = stage;
            if (current != null) BuildHighlight(current);
        }

        if (current == null)
        {
            SetLabels(false);
            return;
        }

        rightLabel.color = canChop ? enabledColor : disabledColor;
        leftLabel.color = enabledColor;
        PlaceLabels(current);
    }

    private void FindTarget(out Transform target, out bool canChop, out int stage)
    {
        target = null; canChop = false; stage = -1;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit[] hits = Physics.RaycastAll(ray, reachDistance, ~0, QueryTriggerInteraction.Ignore);
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            if (holdPoint != null && hit.transform.IsChildOf(holdPoint)) continue; // item in my hand

            Ingredient ing = hit.collider.GetComponentInParent<Ingredient>();
            if (ing != null)
            {
                if (ing.transform.parent != null) continue;   // someone is holding it
                target = ing.transform;
                canChop = ing.CanChop;
                stage = ing.CurrentStage;
                return;
            }

            FoodItem food = hit.collider.GetComponentInParent<FoodItem>();
            if (food != null)
            {
                if (food.transform.parent != null) continue;
                target = food.transform;
                canChop = false;     // cooked meals / drinks can't be cut
                return;
            }

            return; // hit a wall or other object first
        }
    }

    private void BuildHighlight(Transform root)
    {
        foreach (MeshFilter mf in root.GetComponentsInChildren<MeshFilter>())
        {
            MeshRenderer mr = mf.GetComponent<MeshRenderer>();
            if (mf.sharedMesh == null || mr == null || !mr.enabled) continue;

            GameObject go = new GameObject("HoverOverlay");
            go.transform.SetParent(mf.transform, false);
            go.transform.localScale = Vector3.one * overlayScale;

            go.AddComponent<MeshFilter>().sharedMesh = mf.sharedMesh;
            MeshRenderer r = go.AddComponent<MeshRenderer>();
            Material[] mats = new Material[mf.sharedMesh.subMeshCount];
            for (int i = 0; i < mats.Length; i++) mats[i] = highlightMaterial;
            r.sharedMaterials = mats;
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            r.receiveShadows = false;

            overlays.Add(go);
        }
    }

    private void ClearHighlight()
    {
        foreach (GameObject go in overlays)
            if (go != null) Destroy(go);
        overlays.Clear();
        current = null;
        lastStage = -1;
    }

    private void PlaceLabels(Transform root)
    {
        Renderer[] rends = root.GetComponentsInChildren<Renderer>();
        float minX = float.MaxValue, maxX = float.MinValue, minY = float.MaxValue, maxY = float.MinValue;
        bool any = false;

        foreach (Renderer r in rends)
        {
            Bounds b = r.bounds;
            for (int i = 0; i < 8; i++)
            {
                Vector3 corner = b.center + Vector3.Scale(b.extents, new Vector3(
                    (i & 1) == 0 ? -1 : 1, (i & 2) == 0 ? -1 : 1, (i & 4) == 0 ? -1 : 1));
                Vector3 sp = playerCamera.WorldToScreenPoint(corner);
                if (sp.z <= 0f) continue;
                any = true;
                minX = Mathf.Min(minX, sp.x); maxX = Mathf.Max(maxX, sp.x);
                minY = Mathf.Min(minY, sp.y); maxY = Mathf.Max(maxY, sp.y);
            }
        }

        if (!any) { SetLabels(false); return; }

        SetLabels(true);
        float pad = labelPadding * (canvas != null ? canvas.scaleFactor : 1f);
        float midY = (minY + maxY) * 0.5f;
        leftLabel.rectTransform.position = new Vector3(minX - pad, midY, 0f);
        rightLabel.rectTransform.position = new Vector3(maxX + pad, midY, 0f);
    }

    private void SetLabels(bool on)
    {
        leftLabel.gameObject.SetActive(on);
        rightLabel.gameObject.SetActive(on);
    }
}
