using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipRadarUI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public RectTransform radarPanel;
    public Image dotPrefab;

    [Header("Radar Area")]
    public float detectionRadius = 500f;

    [Tooltip("If 0, radius is calculated from the radar panel size.")]
    public float radarRadiusPixels = 0f;

    [Header("Detection")]
    public bool rotateWithPlayer = true;

    private readonly List<Image> dots = new List<Image>();
    private readonly Collider[] detectedColliders = new Collider[256];
    private readonly HashSet<RadarTarget> detectedTargets = new HashSet<RadarTarget>();

    void Awake()
    {
        if (radarPanel == null)
            radarPanel = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (player == null || radarPanel == null || dotPrefab == null)
            return;

        UpdateRadar();
    }

    void UpdateRadar()
    {
        detectedTargets.Clear();

        int count = Physics.OverlapSphereNonAlloc(
            player.position,
            detectionRadius,
            detectedColliders,
            Physics.AllLayers,
            QueryTriggerInteraction.Collide
        );

        int visibleDotCount = 0;

        for (int i = 0; i < count; i++)
        {
            Collider detectedCollider = detectedColliders[i];

            if (detectedCollider == null)
                continue;

            RadarTarget radarTarget =
                detectedCollider.GetComponentInParent<RadarTarget>();

            if (radarTarget == null)
                continue;

            if (detectedTargets.Contains(radarTarget))
                continue;

            detectedTargets.Add(radarTarget);

            if (radarTarget.transform == player)
                continue;

            Vector3 offset = radarTarget.transform.position - player.position;
            Vector2 radarPosition = WorldOffsetToRadarPosition(offset);

            if (radarPosition.magnitude > GetRadarRadiusPixels())
                continue;

            Image dot = GetDot(visibleDotCount);
            dot.gameObject.SetActive(true);

            RectTransform dotRect = dot.GetComponent<RectTransform>();
            dotRect.anchoredPosition = radarPosition;
            dotRect.sizeDelta = new Vector2(radarTarget.dotSize, radarTarget.dotSize);

            dot.color = radarTarget.radarColor;

            visibleDotCount++;
        }

        HideUnusedDots(visibleDotCount);
    }

    Vector2 WorldOffsetToRadarPosition(Vector3 worldOffset)
    {
        Vector3 localOffset = worldOffset;

        if (rotateWithPlayer)
            localOffset = Quaternion.Inverse(GetPlayerYawRotation()) * worldOffset;

        Vector2 flatOffset = new Vector2(localOffset.x, localOffset.z);

        if (flatOffset.sqrMagnitude < 0.001f)
            return Vector2.zero;

        float normalizedDistance = flatOffset.magnitude / detectionRadius;
        float radarDistance = normalizedDistance * GetRadarRadiusPixels();

        return flatOffset.normalized * radarDistance;
    }

    Quaternion GetPlayerYawRotation()
    {
        Vector3 euler = player.rotation.eulerAngles;
        return Quaternion.Euler(0f, euler.y, 0f);
    }

    float GetRadarRadiusPixels()
    {
        if (radarRadiusPixels > 0f)
            return radarRadiusPixels;

        float width = radarPanel.rect.width;
        float height = radarPanel.rect.height;

        return Mathf.Min(width, height) * 0.5f;
    }

    Image GetDot(int index)
    {
        while (dots.Count <= index)
        {
            Image newDot = Instantiate(dotPrefab, radarPanel);
            RectTransform rect = newDot.GetComponent<RectTransform>();

            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);

            dots.Add(newDot);
        }

        return dots[index];
    }

    void HideUnusedDots(int usedCount)
    {
        for (int i = usedCount; i < dots.Count; i++)
        {
            dots[i].gameObject.SetActive(false);
        }
    }
}
