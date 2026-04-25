using UnityEngine;

public class AimingSystem : MonoBehaviour
{
    [Header("Beam")]
    public Transform beamOrigin;
    public float maxDistance = 80f;
    public float startWidth = 0.08f;
    public float endWidth = 0.0f;

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

        Vector3 start = beamOrigin.position;
        Vector3 direction = beamOrigin.forward;
        Vector3 end = start + direction * maxDistance;

        bool hasHit = false;

        RaycastHit hit;

        if (Physics.Raycast(start, direction, out hit, maxDistance, hitMask))
        {
            if (ignoreRoot == null || !hit.transform.IsChildOf(ignoreRoot))
            {
                hasHit = true;
                end = hit.point;
            }
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
