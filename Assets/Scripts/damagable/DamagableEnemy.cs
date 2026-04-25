using UnityEngine;

public class DamagableEnemy : Damageable
{
    [SerializeField] public EnemyOrchestrator master;
    [SerializeField] public int level =1;
    [SerializeField] public int ident;
    void Awake()
    {
        this.maxHealth = maxHealth*level;
        this.health = maxHealth;
        base.Awake();
    }
    public override void Die()
    {
        master.deregister(ident);
        base.Die();
    }
}
