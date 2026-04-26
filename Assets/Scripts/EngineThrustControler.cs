using UnityEngine;

public class EngineThrustControler : MonoBehaviour
{
    [Header("Cone")]
    [SerializeField] private Transform coneTransform;
    [SerializeField] private MeshFilter coneMeshFilter;
    [SerializeField] private MeshRenderer coneRenderer;

    [SerializeField] private float minConeLength = 0.2f;
    [SerializeField] private float maxConeLength = 2.5f;
    [SerializeField] private float boostConeLength = 3.5f;

    [SerializeField] private float minConeRadius = 0.05f;
    [SerializeField] private float maxConeRadius = 0.45f;
    [SerializeField] private float boostConeRadius = 0.65f;

    [SerializeField] private Color coneColor = new Color(0.2f, 0.7f, 1f, 0.3f);

    [SerializeField] private float minConeAlpha = 0f;
    [SerializeField] private float maxConeAlpha = 0.35f;
    [SerializeField] private float boostConeAlpha = 0.5f;

    [Header("Particles")]
    [SerializeField] private ParticleSystem thrustParticles;

    [SerializeField] private float minParticleEmission = 0f;
    [SerializeField] private float maxParticleEmission = 120f;
    [SerializeField] private float boostParticleEmission = 220f;

    [SerializeField] private float minParticleSpeed = 1f;
    [SerializeField] private float maxParticleSpeed = 6f;
    [SerializeField] private float boostParticleSpeed = 10f;

    [Header("Light")]
    [SerializeField] private Light thrustLight;

    [SerializeField] private float minLightIntensity = 0f;
    [SerializeField] private float maxLightIntensity = 4f;
    [SerializeField] private float boostLightIntensity = 7f;

    [Header("Smoothing")]
    [SerializeField] private float changeSpeed = 10f;

    [Header("Audio")]
    [SerializeField] private AudioSource engineAudioSource;

    [SerializeField] private float minEngineVolume = 0f;
    [SerializeField] private float maxEngineVolume = 0.6f;
    [SerializeField] private float boostEngineVolume = 0.9f;

    private float targetThrust;
    private float currentThrust;

    private MaterialPropertyBlock propertyBlock;

    private void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();

        if (coneMeshFilter != null)
        {
            coneMeshFilter.mesh = CreateConeMesh(32);
        }

        if (thrustParticles != null)
        {
            thrustParticles.Play();
        }

        if (engineAudioSource != null)
        {
            engineAudioSource.loop = true;
            engineAudioSource.volume = minEngineVolume;

            if (!engineAudioSource.isPlaying)
                engineAudioSource.Play();
        }
    }

    private void Update()
    {
        currentThrust = Mathf.Lerp(
            currentThrust,
            targetThrust,
            Time.deltaTime * changeSpeed
        );

        UpdateCone();
        UpdateParticles();
        UpdateLight();
        UpdateAudio();
    }

    private void UpdateAudio()
    {
        if (engineAudioSource == null)
        {
            return;
        }

        float normalThrust = Mathf.Clamp01(currentThrust);
        float boostThrust = Mathf.InverseLerp(1f, 1.4f, currentThrust);

        float volume = Mathf.Lerp(
            minEngineVolume,
            maxEngineVolume,
            normalThrust
        );

        volume = Mathf.Lerp(
            volume,
            boostEngineVolume,
            boostThrust
        );

        engineAudioSource.volume = volume;
    }

    public void SetThrust(float thrust)
    {
        targetThrust = Mathf.Clamp(thrust, 0f, 1.4f);
    }

    private void UpdateCone()
    {
        if (coneTransform == null || coneRenderer == null)
        {
            return;
        }

        float normalThrust = Mathf.Clamp01(currentThrust);
        float boostThrust = Mathf.InverseLerp(1f, 1.4f, currentThrust);

        float length = Mathf.Lerp(minConeLength, maxConeLength, normalThrust);
        length = Mathf.Lerp(length, boostConeLength, boostThrust);

        float radius = Mathf.Lerp(minConeRadius, maxConeRadius, normalThrust);
        radius = Mathf.Lerp(radius, boostConeRadius, boostThrust);

        float alpha = Mathf.Lerp(minConeAlpha, maxConeAlpha, normalThrust);
        alpha = Mathf.Lerp(alpha, boostConeAlpha, boostThrust);

        coneTransform.localScale = new Vector3(radius, radius, length);

        Color finalColor = coneColor;
        finalColor.a = alpha;

        coneRenderer.GetPropertyBlock(propertyBlock);

        propertyBlock.SetColor("_BaseColor", finalColor); // URP
        propertyBlock.SetColor("_Color", finalColor);     // Built-in
        propertyBlock.SetColor("_EmissionColor", finalColor * 2f);

        coneRenderer.SetPropertyBlock(propertyBlock);

        coneRenderer.enabled = currentThrust > 0.01f;
    }

    private void UpdateParticles()
    {
        if (thrustParticles == null)
        {
            return;
        }

        float normalThrust = Mathf.Clamp01(currentThrust);
        float boostThrust = Mathf.InverseLerp(1f, 1.4f, currentThrust);

        float emissionAmount = Mathf.Lerp(
            minParticleEmission,
            maxParticleEmission,
            normalThrust
        );

        emissionAmount = Mathf.Lerp(
            emissionAmount,
            boostParticleEmission,
            boostThrust
        );

        float speedAmount = Mathf.Lerp(
            minParticleSpeed,
            maxParticleSpeed,
            normalThrust
        );

        speedAmount = Mathf.Lerp(
            speedAmount,
            boostParticleSpeed,
            boostThrust
        );

        ParticleSystem.EmissionModule emission = thrustParticles.emission;
        emission.rateOverTime = emissionAmount;

        ParticleSystem.MainModule main = thrustParticles.main;
        main.startSpeed = speedAmount;
    }

    private void UpdateLight()
    {
        if (thrustLight == null)
        {
            return;
        }

        float normalThrust = Mathf.Clamp01(currentThrust);
        float boostThrust = Mathf.InverseLerp(1f, 1.4f, currentThrust);

        float intensity = Mathf.Lerp(
            minLightIntensity,
            maxLightIntensity,
            normalThrust
        );

        intensity = Mathf.Lerp(
            intensity,
            boostLightIntensity,
            boostThrust
        );

        thrustLight.intensity = intensity;
        thrustLight.enabled = currentThrust > 0.01f;
    }

    private Mesh CreateConeMesh(int segments)
    {
        Mesh mesh = new Mesh();
        mesh.name = "Engine Thrust Cone";

        Vector3[] vertices = new Vector3[segments + 1];
        int[] triangles = new int[segments * 3];

        vertices[0] = Vector3.zero;

        for (int i = 0; i < segments; i++)
        {
            float angle = ( (float) i / segments ) * Mathf.PI * 2f;

            vertices[i + 1] = new Vector3(
                Mathf.Cos(angle),
                Mathf.Sin(angle),
                -1f
            );
        }

        for (int i = 0; i < segments; i++)
        {
            int next = ( i + 1 ) % segments;

            triangles[i * 3 + 0] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = next + 1;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
}
