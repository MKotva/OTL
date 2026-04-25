using UnityEngine;

public class ShieldSector : MonoBehaviour
{
    [Header("Energy")]
    public float maxEnergy = 100f;
    public float energy = 100f;
    public float rechargeRate = 6f;

    [Header("Collapse")]
    public float collapseEnergy = 0f;
    public float reactivateEnergy = 20f;

    [Tooltip("Keep false if you still want empty shields to flash white when hit.")]
    public bool disableColliderWhenCollapsed = false;

    [Header("Hit")]
    public float defaultHitDamage = 10f;
    public GameObject hitEffectPrefab;
    public float hitEffectLifetime = 2f;

    [Header("References")]
    public Collider shieldCollider;
    public Material shieldMaterial;

    [Header("Calm Visual")]
    [Range(0f, 1f)]
    public float calmAlpha = 0.015f;

    public Color calmFullEnergyColor = new Color(0.0f, 0.03f, 0.25f, 1f);
    public Color calmEmptyEnergyColor = new Color(0.25f, 0.0f, 0.0f, 1f);

    [Header("Hit Flash Visual")]
    public float hitFlashDuration = 0.25f;

    [Range(0f, 1f)]
    public float hitAlpha = 0.65f;

    public Color hitFullEnergyColor = new Color(0.25f, 0.85f, 1f, 1f);
    public Color hitEmptyEnergyColor = new Color(1f, 0.12f, 0.05f, 1f);

    [Header("Empty Shield Flash")]
    public float emptyFlashDuration = 0.14f;

    [Range(0f, 1f)]
    public float emptyFlashAlpha = 1f;

    public Color emptyFlashColor = Color.white;

    [Header("Emission")]
    public float calmEmissionStrength = 0.2f;
    public float hitEmissionStrength = 2.5f;
    public float emptyFlashEmissionStrength = 5f;

    private float flashTimer;
    private float flashDuration;
    private bool emptyFlash;
    private bool collapsed;

    public bool IsCollapsed
    {
        get { return collapsed; }
    }

    public float Energy01
    {
        get
        {
            if (maxEnergy <= 0f)
                return 0f;

            return Mathf.Clamp01(energy / maxEnergy);
        }
    }

    void Awake()
    {
        if (shieldCollider == null)
            shieldCollider = GetComponent<Collider>();

        if (shieldMaterial == null)
        {
            Renderer renderer = GetComponent<Renderer>();

            if (renderer != null)
                shieldMaterial = renderer.material;
        }

        energy = Mathf.Clamp(energy, 0f, maxEnergy);

        UpdateShieldState();
        UpdateVisual();
    }

    void Update()
    {
        Recharge(Time.deltaTime);
        UpdateFlash(Time.deltaTime);
        UpdateVisual();
    }

    public void Recharge(float deltaTime)
    {
        if (energy >= maxEnergy)
            return;

        energy += rechargeRate * deltaTime;
        energy = Mathf.Clamp(energy, 0f, maxEnergy);

        UpdateShieldState();
    }

    public void AddEnergy(float amount)
    {
        if (amount <= 0f)
            return;

        energy += amount;
        energy = Mathf.Clamp(energy, 0f, maxEnergy);

        UpdateShieldState();
    }

    public float RemoveEnergy(float amount)
    {
        if (amount <= 0f)
            return 0f;

        float removed = Mathf.Min(energy, amount);
        energy -= removed;

        UpdateShieldState();

        return removed;
    }

    public bool TakeDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        SpawnHitEffect(hitPoint, hitNormal);

        if (collapsed || energy <= collapseEnergy)
        {
            StartEmptyFlash();
            return false;
        }

        RemoveEnergy(damage);

        if (energy <= collapseEnergy)
            StartEmptyFlash();
        else
            StartHitFlash();

        return true;
    }

    void StartHitFlash()
    {
        emptyFlash = false;
        flashDuration = hitFlashDuration;
        flashTimer = flashDuration;
    }

    void StartEmptyFlash()
    {
        emptyFlash = true;
        flashDuration = emptyFlashDuration;
        flashTimer = flashDuration;
    }

    void UpdateFlash(float deltaTime)
    {
        if (flashTimer <= 0f)
            return;

        flashTimer -= deltaTime;

        if (flashTimer < 0f)
            flashTimer = 0f;
    }

    void UpdateShieldState()
    {
        if (!collapsed && energy <= collapseEnergy)
            collapsed = true;

        if (collapsed && energy >= reactivateEnergy)
            collapsed = false;

        if (shieldCollider != null && disableColliderWhenCollapsed)
            shieldCollider.enabled = !collapsed;
    }

    void UpdateVisual()
    {
        float energy01 = Energy01;

        Color calmColor = Color.Lerp(
            calmEmptyEnergyColor,
            calmFullEnergyColor,
            energy01
        );

        Color hitColor = Color.Lerp(
            hitEmptyEnergyColor,
            hitFullEnergyColor,
            energy01
        );

        Color finalColor = calmColor;
        float finalAlpha = calmAlpha;
        float emissionStrength = calmEmissionStrength;

        if (collapsed && flashTimer <= 0f)
        {
            finalAlpha = 0f;
            emissionStrength = 0f;
        }

        if (flashTimer > 0f && flashDuration > 0f)
        {
            float flash01 = flashTimer / flashDuration;
            float flashStrength = flash01;

            if (emptyFlash)
            {
                finalColor = Color.Lerp(calmColor, emptyFlashColor, flashStrength);
                finalAlpha = Mathf.Lerp(0f, emptyFlashAlpha, flashStrength);

                emissionStrength = Mathf.Lerp(
                    0f,
                    emptyFlashEmissionStrength,
                    flashStrength
                );
            }
            else
            {
                finalColor = Color.Lerp(calmColor, hitColor, flashStrength);
                finalAlpha = Mathf.Lerp(calmAlpha, hitAlpha, flashStrength);

                emissionStrength = Mathf.Lerp(
                    calmEmissionStrength,
                    hitEmissionStrength,
                    flashStrength
                );
            }
        }

        finalColor.a = finalAlpha;

        ApplyMaterialColor(finalColor, emissionStrength);
    }

    void ApplyMaterialColor(Color color, float emissionStrength)
    {
        if (shieldMaterial == null)
            return;

        if (shieldMaterial.HasProperty("_BaseColor"))
            shieldMaterial.SetColor("_BaseColor", color);

        if (shieldMaterial.HasProperty("_Color"))
            shieldMaterial.SetColor("_Color", color);

        Color emissionColor = new Color(
            color.r * emissionStrength,
            color.g * emissionStrength,
            color.b * emissionStrength,
            1f
        );

        if (shieldMaterial.HasProperty("_EmissionColor"))
            shieldMaterial.SetColor("_EmissionColor", emissionColor);
    }

    void SpawnHitEffect(Vector3 hitPoint, Vector3 hitNormal)
    {
        if (hitEffectPrefab == null)
            return;

        Quaternion rotation = Quaternion.identity;

        if (hitNormal.sqrMagnitude > 0.001f)
            rotation = Quaternion.LookRotation(hitNormal.normalized);

        GameObject effect = Instantiate(hitEffectPrefab, hitPoint, rotation);

        if (hitEffectLifetime > 0f)
            Destroy(effect, hitEffectLifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        Vector3 hitPoint = GetTriggerHitPoint(other);
        Vector3 hitNormal = GetHitNormal(hitPoint);

        HandleHit(other.gameObject, other, hitPoint, hitNormal);
    }

    void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.GetContact(0);

        HandleHit(
            collision.gameObject,
            collision.collider,
            contact.point,
            contact.normal
        );
    }

    void HandleHit(
        GameObject hitObject,
        Collider hitCollider,
        Vector3 hitPoint,
        Vector3 hitNormal
    )
    {
        ShieldDamageController damageSource =
            hitObject.GetComponentInParent<ShieldDamageController>();

        if (damageSource == null)
            return;

        if (damageSource.ShouldIgnoreShield(this, shieldCollider))
            return;

        bool absorbed = TakeDamage(
            damageSource.shieldDamage,
            hitPoint,
            hitNormal
        );

       // damageSource.OnShieldHit(this, absorbed);
    }

    Vector3 GetTriggerHitPoint(Collider other)
    {
        if (shieldCollider != null)
            return shieldCollider.ClosestPoint(other.bounds.center);

        return other.bounds.center;
    }

    Vector3 GetHitNormal(Vector3 hitPoint)
    {
        Vector3 normal = hitPoint - transform.position;

        if (normal.sqrMagnitude < 0.001f)
            return transform.forward;

        return normal.normalized;
    }
}