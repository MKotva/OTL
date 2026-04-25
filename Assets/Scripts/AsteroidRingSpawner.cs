using UnityEngine;

public class AsteroidRingSpawner : MonoBehaviour
{
    [Header("Asteroid Prefabs")]
    public GameObject[] asteroidPrefabs;

    [Header("Ring Area")]
    public float innerRadius = 500f;
    public float outerRadius = 700f;
    public float heightRange = 200f;

    [Header("Groups")]
    public int groupCount = 20;
    public int minAsteroidsPerGroup = 5;
    public int maxAsteroidsPerGroup = 20;
    public float groupRadius = 45f;

    [Header("Scale")]
    public float minScale = 0.5f;
    public float maxScale = 10f;

    [Tooltip("Higher value creates more small asteroids and fewer huge ones.")]
    public float scaleBias = 2f;

    [Header("Rigidbody Mass")]
    public float baseMass = 10f;
    public float minMass = 1f;
    public float maxMass = 5000f;

    [Header("Motion")]
    public float minDriftSpeed = 0.1f;
    public float maxDriftSpeed = 1.5f;
    public float groupDriftStrength = 0.7f;

    public float minRotationSpeed = 2f;
    public float maxRotationSpeed = 25f;

    [Header("Shield Damage Defaults")]
    public bool setupShieldDamage = true;

    [Tooltip("If false, asteroid prefabs keep their own ShieldDamageController settings.")]
    public bool overridePrefabDamageSettings = false;

    public float defaultBaseShieldDamage = 10f;
    public float defaultScaleDamagePower = 1.5f;
    public float defaultMinShieldDamage = 5f;
    public float defaultMaxShieldDamage = 150f;

    [Header("Randomness")]
    public bool useSeed = false;
    public int seed = 12345;

    [Header("Generation")]
    public bool generateOnStart = true;

    void Start()
    {
        if (generateOnStart)
            GenerateAsteroidRing();
    }

    [ContextMenu("Generate Asteroid Ring")]
    public void GenerateAsteroidRing()
    {
        if (asteroidPrefabs == null || asteroidPrefabs.Length == 0)
        {
            Debug.LogWarning("No asteroid prefabs assigned.");
            return;
        }

        ClearAsteroids();

        if (useSeed)
            Random.InitState(seed);

        for (int groupIndex = 0; groupIndex < groupCount; groupIndex++)
        {
            Vector3 groupCenter = GetRandomPointInRing();

            Vector3 groupVelocity =
                Random.onUnitSphere *
                Random.Range(minDriftSpeed, maxDriftSpeed) *
                groupDriftStrength;

            int asteroidCount = Random.Range(
                minAsteroidsPerGroup,
                maxAsteroidsPerGroup + 1
            );

            for (int i = 0; i < asteroidCount; i++)
            {
                SpawnAsteroidInGroup(groupCenter, groupVelocity);
            }
        }
    }

    void SpawnAsteroidInGroup(Vector3 groupCenter, Vector3 groupVelocity)
    {
        GameObject prefab = asteroidPrefabs[Random.Range(0, asteroidPrefabs.Length)];

        Vector3 position = GetRandomPointNearGroup(groupCenter);
        Quaternion rotation = Random.rotation;

        GameObject asteroid = Instantiate(prefab, position, rotation, transform);
        asteroid.name = prefab.name + "_Asteroid";

        float scaleT = Mathf.Pow(Random.value, scaleBias);
        float scale = Mathf.Lerp(minScale, maxScale, scaleT);

        asteroid.transform.localScale *= scale;

        SetupRigidbody(asteroid, scale);
        SetupMotion(asteroid, groupVelocity);
        SetupShieldDamage(asteroid, scale);
    }

    void SetupRigidbody(GameObject asteroid, float scale)
    {
        Rigidbody rb = asteroid.GetComponent<Rigidbody>();

        if (rb == null)
            return;

        rb.useGravity = false;

        float mass = baseMass * scale * scale * scale;
        rb.mass = Mathf.Clamp(mass, minMass, maxMass);
    }

    void SetupMotion(GameObject asteroid, Vector3 groupVelocity)
    {
        AsteroidMotionController motion = asteroid.GetComponent<AsteroidMotionController>();

        if (motion == null)
            motion = asteroid.AddComponent<AsteroidMotionController>();

        Vector3 individualVelocity =
            Random.onUnitSphere *
            Random.Range(minDriftSpeed, maxDriftSpeed);

        motion.velocity = groupVelocity + individualVelocity;
        motion.rotationAxis = Random.onUnitSphere;
        motion.rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);

        motion.ApplyMotion();
    }

    void SetupShieldDamage(GameObject asteroid, float scale)
    {
        if (!setupShieldDamage)
            return;

        ShieldDamageController damageSource =
            asteroid.GetComponent<ShieldDamageController>();

        bool addedDamageSource = false;

        if (damageSource == null)
        {
            damageSource = asteroid.AddComponent<ShieldDamageController>();
            addedDamageSource = true;
        }

        if (addedDamageSource || overridePrefabDamageSettings)
        {
            damageSource.autoCalculateDamageFromScale = true;
            damageSource.baseShieldDamage = defaultBaseShieldDamage;
            damageSource.scaleDamagePower = defaultScaleDamagePower;
            damageSource.minShieldDamage = defaultMinShieldDamage;
            damageSource.maxShieldDamage = defaultMaxShieldDamage;
        }

        damageSource.destroyOnShieldHit = false;
        damageSource.destroyOnlyWhenShieldAbsorbs = false;

        if (damageSource.autoCalculateDamageFromScale)
            damageSource.RecalculateDamage(scale);
    }

    Vector3 GetRandomPointNearGroup(Vector3 groupCenter)
    {
        for (int attempt = 0; attempt < 20; attempt++)
        {
            Vector3 localOffset = Random.insideUnitSphere * groupRadius;
            Vector3 worldPosition = groupCenter + localOffset;

            if (IsInsideRing(worldPosition))
                return worldPosition;
        }

        return groupCenter;
    }

    Vector3 GetRandomPointInRing()
    {
        float innerSquared = innerRadius * innerRadius;
        float outerSquared = outerRadius * outerRadius;

        float radius = Mathf.Sqrt(Random.Range(innerSquared, outerSquared));
        float angle = Random.Range(0f, Mathf.PI * 2f);

        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;
        float y = Random.Range(-heightRange * 0.5f, heightRange * 0.5f);

        Vector3 localPosition = new Vector3(x, y, z);

        return transform.TransformPoint(localPosition);
    }

    bool IsInsideRing(Vector3 worldPosition)
    {
        Vector3 localPosition = transform.InverseTransformPoint(worldPosition);

        float distance = new Vector2(
            localPosition.x,
            localPosition.z
        ).magnitude;

        if (distance < innerRadius)
            return false;

        if (distance > outerRadius)
            return false;

        if (Mathf.Abs(localPosition.y) > heightRange * 0.5f)
            return false;

        return true;
    }

    [ContextMenu("Clear Asteroids")]
    public void ClearAsteroids()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            if (Application.isPlaying)
                Destroy(transform.GetChild(i).gameObject);
            else
                DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    void OnDrawGizmosSelected()
    {
        DrawCircle(innerRadius, Color.yellow);
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