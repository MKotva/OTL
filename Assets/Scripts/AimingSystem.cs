using UnityEngine;

public class AimingSystem : MonoBehaviour
{
    [Header("Beam")]
    public Transform beamOrigin;
    public float maxDistance = 80f;
    public float startWidth = 0.08f;
    public float endWidth = 0.0f;

    // Change this in Inspector if forward is wrong.
    // Try Vector3.forward, Vector3.right, Vector3.up, or negative versions.
    public Vector3 localDirection = Vector3.forward;

    // Moves the beam start slightly forward, so it does not begin inside the ship.
    public float startOffset = 0.1f;

    [Header("Colors")]
    public Color normalColor = new Color(0f, 0.8f, 1f, 0.8f);
    public Color targetColor = new Color(1f, 0.1f, 0.05f, 0.9f);

    [Header("Hit Detection")]
    public LayerMask hitMask = ~0;
    public Transform ignoreRoot;

    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;

        lineRenderer.startWidth = startWidth;
        lineRenderer.endWidth = endWidth;
    }

    void Update()
    {
        if (beamOrigin == null)
            beamOrigin = transform;

        Vector3 direction = beamOrigin.TransformDirection(localDirection.normalized);

        Vector3 start = beamOrigin.position + direction * startOffset;
        Vector3 end = start + direction * maxDistance;

        bool hasHit = false;

        RaycastHit[] hits = Physics.RaycastAll(start, direction, maxDistance, hitMask);

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            if (ignoreRoot != null && hit.transform.IsChildOf(ignoreRoot))
                continue;

            hasHit = true;
            end = hit.point;
            break;
        }

        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        if (hasHit)
            SetBeamColor(targetColor);
        else
            SetBeamColor(normalColor);
    }

    void SetBeamColor(Color color)
    {
        Gradient gradient = new Gradient();

        Color startColor = color;
        Color endColor = color;

        startColor.a = color.a;
        endColor.a = 0f;

        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(startColor, 0f),
                new GradientColorKey(endColor, 1f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(startColor.a, 0f),
                new GradientAlphaKey(0f, 1f)
            }
        );

        lineRenderer.colorGradient = gradient;
    }
}