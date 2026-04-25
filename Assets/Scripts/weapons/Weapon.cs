using Unity.IO.LowLevel.Unsafe;
using UnityEngine;


public class Weapon : MonoBehaviour
{
     [Header("Weapon")]
    [SerializeField] public int damage;
    [SerializeField] public float fireRate;
    [SerializeField] public float velocity;
    [SerializeField] public float duration;
    public WeaponTypes weaponType = WeaponTypes.physical;
    [Header("Impact")]
    public GameObject explosionPrefab;
    public float explosionLifetime = 2f;
    public bool destroyOnHit = true;
    public GameObject projectilePrefab;
    public virtual Projectile configureProjectile(Projectile projectile)
    {

           projectile.speed = velocity;
           projectile.lifeTime = duration;

           projectile.expirePrefab = explosionPrefab;
           projectile.explosionLifetime = explosionLifetime;
           projectile.DamageInstance = new DamageInstance(WeaponTypesDict.DamageSpreads[weaponType],damage);
         //  projectile.targetZScale = projectileTargetZScale;
         //  projectile.growDuration = projectileGrowDuration;
         //  projectile.shrinkDuration = projectileShrinkDuration;
        return projectile;
    }
    public virtual GameObject Fire(Transform firePoint )
    {
        return Instantiate(
            this.projectilePrefab,
            firePoint.position,
            firePoint.rotation
        );
    }
    public virtual GameObject AimedFire(Transform firePoint, Transform target )
    {
        Vector3 direction = target.position - firePoint.position;
        return Instantiate(
            this.projectilePrefab,
            firePoint.position,
            Quaternion.LookRotation(direction)
        );
    }
}
