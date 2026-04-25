using UnityEngine;

public class Damageable : MonoBehaviour
{
    public float maxHealth = 100f;
    public float health = 100f;

    [Header("Destroy")]
    public bool destroyWhenDead = true;
    public GameObject deathEffectPrefab;
    public float deathEffectLifetime = 2f;

    void Awake()
    {
        health = Mathf.Clamp(health, 0f, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0f)
            return;

        health -= damage;

        if (health <= 0f)
            Die();
    }

    void Die()
    {
        if (deathEffectPrefab != null)
        {
            GameObject effect = Instantiate(
                deathEffectPrefab,
                transform.position,
                transform.rotation
            );

            if (deathEffectLifetime > 0f)
                Destroy(effect, deathEffectLifetime);
        }

        if (destroyWhenDead)
            Destroy(gameObject);
    }
}