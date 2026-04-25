using UnityEngine;
[CreateAssetMenu(fileName = "SimpleHit", menuName = "Hit Handlers/SimpleHit")]
public class SimpleHit :  IOnHit
{
    public GameObject explosionPrefab;
    public float explosionLifetime =2f;
    private bool exploded {get;set;}
     public  override void HandleHit(Projectile proj,GameObject hitObject,DamageInstance damInst)
    {
        if (damInst!= null)
            damInst.ApplyDamage(hitObject);

            ExplodeAndDestroy(proj);
    }
    

    public void ExplodeAndDestroy(Projectile proj)
    {
        if (exploded)
            return;

        exploded = true;

        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(
                explosionPrefab,
                proj.gameObject.transform.position,
                Quaternion.identity
            );

            if (explosionLifetime > 0f)
                Destroy(explosion, explosionLifetime);
        }

        Destroy(proj);
    }
}
