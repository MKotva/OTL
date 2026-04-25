using UnityEngine;

public class ShieldDamageController : MonoBehaviour
{
    [Header("Shield Damage")]
    public float shieldDamage = 15f;

    [Header("Scale Damage Optional")]
    public bool autoCalculateDamageFromScale = false;
    public float baseShieldDamage = 10f;
    public float damageMultiplier = 1f;
    public float scaleDamagePower = 1.5f;
    public float minShieldDamage = 5f;
    public float maxShieldDamage = 150f;

    [Header("Ignored Shields")]
    public ShieldSector[] ignoredShields;
    public Collider[] ignoredShieldColliders;

    [Header("Destroy")]
    public bool destroyOnShieldHit = true;
    public bool destroyOnlyWhenShieldAbsorbs = true;

    void Awake()
    {
        if (autoCalculateDamageFromScale)
            RecalculateDamageFromCurrentScale();
    }

    void Start()
    {
        if (autoCalculateDamageFromScale)
            RecalculateDamageFromCurrentScale();
    }

    public void SetIgnoredShields(ShieldSector[] shields)
    {
        ignoredShields = shields;
    }

    public void SetIgnoredShieldColliders(Collider[] colliders)
    {
        ignoredShieldColliders = colliders;
    }

    public bool ShouldIgnoreShield(ShieldSector shieldSector, Collider shieldCollider)
    {
        if (shieldSector != null && ignoredShields != null)
        {
            for (int i = 0; i < ignoredShields.Length; i++)
            {
                if (ignoredShields[i] == shieldSector)
                    return true;
            }
        }

        if (shieldCollider != null && ignoredShieldColliders != null)
        {
            for (int i = 0; i < ignoredShieldColliders.Length; i++)
            {
                if (ignoredShieldColliders[i] == shieldCollider)
                    return true;
            }
        }

        return false;
    }

    public void RecalculateDamageFromCurrentScale()
    {
        float scale = GetAverageWorldScale();
        RecalculateDamage(scale);
    }

    public void RecalculateDamage(float scale)
    {
        float damage = baseShieldDamage * damageMultiplier;
        damage *= Mathf.Pow(Mathf.Max(scale, 0.01f), scaleDamagePower);

        shieldDamage = Mathf.Clamp(
            damage,
            minShieldDamage,
            maxShieldDamage
        );
    }

    float GetAverageWorldScale()
    {
        Vector3 scale = transform.lossyScale;
        return ( scale.x + scale.y + scale.z ) / 3f;
    }

    public void OnShieldHit(ShieldSector shieldSector, bool absorbed)
    {
        if (!destroyOnShieldHit)
            return;

        if (destroyOnlyWhenShieldAbsorbs && !absorbed)
            return;

        ProjectileController projectile = GetComponent<ProjectileController>();

        if (projectile != null)
            projectile.ExplodeAndDestroy();
        else
            Destroy(gameObject);
    }
}