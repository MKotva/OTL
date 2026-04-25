using TMPro;
using UnityEngine;

public class SpaceBoundaries : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Map Radius")]
    public float innerRadius = 500f;
    public float outerRadius = 700f;

    [Header("Deep Space Warning")]
    public float destructionCountdown = 10f;
    public bool destroyImmediatelyOutsideOuterRadius = true;

    [Header("UI")]
    public GameObject warningPanel;
    public TMP_Text warningText;

    private float currentCountdown;
    private bool playerIsInDeepSpace;

    void Start()
    {
        currentCountdown = destructionCountdown;
        SetWarningVisible(false);
    }

    void Update()
    {
        if (player == null)
            return;

        float distanceFromCenter = GetPlanarDistanceFromCenter(player.position);

        if (distanceFromCenter <= innerRadius)
        {
            playerIsInDeepSpace = false;
            currentCountdown = destructionCountdown;
            SetWarningVisible(false);
            return;
        }

        playerIsInDeepSpace = true;
        currentCountdown -= Time.deltaTime;

        SetWarningVisible(true);
        UpdateWarningText(distanceFromCenter);

        if (currentCountdown <= 0f)
        {
            DestroyPlayer();
            return;
        }

        if (destroyImmediatelyOutsideOuterRadius && distanceFromCenter >= outerRadius)
        {
            DestroyPlayer();
            return;
        }
    }

    float GetPlanarDistanceFromCenter(Vector3 worldPosition)
    {
        Vector3 localPosition = transform.InverseTransformPoint(worldPosition);

        // XZ plane distance, so height does not matter
        Vector2 flatPosition = new Vector2(localPosition.x, localPosition.z);

        return flatPosition.magnitude;
    }

    void SetWarningVisible(bool visible)
    {
        if (warningPanel != null)
            warningPanel.SetActive(visible);
    }

    void UpdateWarningText(float distanceFromCenter)
    {
        if (warningText == null)
            return;

        warningText.text =
            "WARNING: DEEP SPACE\n" +
            "Return to the inner zone\n" +
            "Destruction in " + Mathf.CeilToInt(currentCountdown) + "s";
    }

    void DestroyPlayer()
    {
        SetWarningVisible(false);

        if (player != null)
            Destroy(player.gameObject);
    }

    void OnDrawGizmosSelected()
    {
        DrawCircle(innerRadius, Color.green);
        DrawCircle(outerRadius, Color.red);
    }

    void DrawCircle(float radius, Color color)
    {
        Gizmos.color = color;

        int segments = 96;
        Vector3 previousPoint = transform.position + transform.right * radius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = i / (float) segments * Mathf.PI * 2f;

            Vector3 localPoint = new Vector3(
                Mathf.Cos(angle) * radius,
                0f,
                Mathf.Sin(angle) * radius
            );

            Vector3 worldPoint = transform.TransformPoint(localPoint);

            Gizmos.DrawLine(previousPoint, worldPoint);
            previousPoint = worldPoint;
        }
    }
}
