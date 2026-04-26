using System;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    public float maxHealth = 1000f;
    public float health = 1000f;
    public Armour armour {get;set;} = new Armour();
    [Header("Destroy")]
    public bool destroyWhenDead = true;
    public GameObject deathEffectPrefab;
    public float deathEffectLifetime = 2f;
    private bool dead = false;

    public bool IsDead
    {
        get { return dead; }
    }

    public void Awake()
    {
        //health = Mathf.Clamp(health, 0f, maxHealth);
    }

    public void TakeDamage(DamageInstance damage)
    {  
        
        
        if (damage.DamageAmount <= 0f)
            return; 
        health -= Mathf.Max((damage.DamageAmount*damage.DamageSpread.PhysicalDamage)-armour.PhysicalResistance,0f);
        health -= Mathf.Max((damage.DamageAmount*damage.DamageSpread.ThermalDamage)-armour.ThermalResistance,0f);
        health -= Mathf.Max((damage.DamageAmount*damage.DamageSpread.ShockDamage)-armour.ShockResistance,0f);
        Debug.Log(health);
        if (health <= 0f)
            Die();
    }

    public virtual void Die()
    {
        if (!dead)
        {
             dispose(); 
        }
        dead=true;
      
    }
    public void dispose()
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
        {
            Debug.Log("You died");
            gameObject.SetActive(false);
            Destroy(gameObject);

        }
    
        
    }
}