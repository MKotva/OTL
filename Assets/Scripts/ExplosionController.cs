using UnityEngine;
using UnityEngine.Rendering;

public class ExplosionController : MonoBehaviour
{
    [Header("Particles")]
    [SerializeField] private ParticleSystem[] particleSystems;
    [SerializeField] private bool findParticlesInChildren = true;
    [SerializeField] private bool forceRuntimeMaterial = true;
    [SerializeField] private bool additiveMaterial = true;

    [Header("Particle Colors")]
    [SerializeField] private Color startColor = new Color(1f, 1f, 0.8f, 1f);
    [SerializeField] private Color middleColor = new Color(1f, 0.35f, 0f, 0.85f);
    [SerializeField] private Color endColor = new Color(0.35f, 0f, 0f, 0f);

    [Header("Light")]
    [SerializeField] private Light explosionLight;
    [SerializeField] private float startLightIntensity = 15f;
    [SerializeField] private float lightFadeSpeed = 8f;

    [Header("Lifetime")]
    [SerializeField] private float destroyAfterSeconds = 3f;

    private void Awake()
    {
        if (findParticlesInChildren)
        {
            particleSystems = GetComponentsInChildren<ParticleSystem>(true);
        }

        if (forceRuntimeMaterial)
        {
            ApplyRuntimeParticleMaterials();
        }

        ApplyParticleColors();
        ApplyLight();
    }

    private void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }

    private void Update()
    {
        FadeLight();
    }

    private void ApplyRuntimeParticleMaterials()
    {
        Shader shader = FindParticleShader();

        if (shader == null)
        {
            Debug.LogError("No valid particle shader found. Your particles will stay pink.");
            return;
        }

        for (int i = 0; i < particleSystems.Length; i++)
        {
            if (particleSystems[i] == null)
                continue;

            ParticleSystemRenderer particleRenderer =
                particleSystems[i].GetComponent<ParticleSystemRenderer>();

            if (particleRenderer == null)
                continue;

            Material material = new Material(shader);
            material.name = "Runtime Explosion Particle Material";

            SetMaterialWhite(material);
            SetupTransparentMaterial(material);

            particleRenderer.material = material;
        }
    }

    private Shader FindParticleShader()
    {
        string[] shaderNames =
        {
            "Universal Render Pipeline/Particles/Unlit",
            "Particles/Standard Unlit",
            "Legacy Shaders/Particles/Additive",
            "Sprites/Default"
        };

        for (int i = 0; i < shaderNames.Length; i++)
        {
            Shader shader = Shader.Find(shaderNames[i]);

            if (shader != null)
                return shader;
        }

        return null;
    }

    private void SetMaterialWhite(Material material)
    {
        if (material.HasProperty("_BaseColor"))
            material.SetColor("_BaseColor", Color.white);

        if (material.HasProperty("_Color"))
            material.SetColor("_Color", Color.white);

        if (material.HasProperty("_TintColor"))
            material.SetColor("_TintColor", Color.white);
    }

    private void SetupTransparentMaterial(Material material)
    {
        material.renderQueue = (int) RenderQueue.Transparent;

        if (material.HasProperty("_Surface"))
            material.SetFloat("_Surface", 1f);

        if (material.HasProperty("_Blend"))
            material.SetFloat("_Blend", additiveMaterial ? 2f : 0f);

        if (material.HasProperty("_ZWrite"))
            material.SetFloat("_ZWrite", 0f);

        if (material.HasProperty("_SrcBlend"))
            material.SetFloat("_SrcBlend", additiveMaterial ? (float) BlendMode.One : (float) BlendMode.SrcAlpha);

        if (material.HasProperty("_DstBlend"))
            material.SetFloat("_DstBlend", additiveMaterial ? (float) BlendMode.One : (float) BlendMode.OneMinusSrcAlpha);

        material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        material.EnableKeyword("_ALPHABLEND_ON");
    }

    private void ApplyParticleColors()
    {
        Gradient gradient = CreateExplosionGradient();

        for (int i = 0; i < particleSystems.Length; i++)
        {
            if (particleSystems[i] == null)
                continue;

            ParticleSystem.MainModule main = particleSystems[i].main;
            main.startColor = Color.white;

            ParticleSystem.ColorOverLifetimeModule colorOverLifetime =
                particleSystems[i].colorOverLifetime;

            colorOverLifetime.enabled = true;
            colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);

            if (!particleSystems[i].isPlaying)
                particleSystems[i].Play();
        }
    }

    private Gradient CreateExplosionGradient()
    {
        Gradient gradient = new Gradient();

        GradientColorKey[] colorKeys =
        {
            new GradientColorKey(startColor, 0f),
            new GradientColorKey(middleColor, 0.35f),
            new GradientColorKey(endColor, 1f)
        };

        GradientAlphaKey[] alphaKeys =
        {
            new GradientAlphaKey(startColor.a, 0f),
            new GradientAlphaKey(middleColor.a, 0.35f),
            new GradientAlphaKey(endColor.a, 1f)
        };

        gradient.SetKeys(colorKeys, alphaKeys);

        return gradient;
    }

    private void ApplyLight()
    {
        if (explosionLight == null)
            return;

        explosionLight.color = middleColor;
        explosionLight.intensity = startLightIntensity;
        explosionLight.enabled = true;
    }

    private void FadeLight()
    {
        if (explosionLight == null)
            return;

        explosionLight.intensity = Mathf.Lerp(
            explosionLight.intensity,
            0f,
            lightFadeSpeed * Time.deltaTime
        );

        if (explosionLight.intensity < 0.05f)
            explosionLight.enabled = false;
    }
}

