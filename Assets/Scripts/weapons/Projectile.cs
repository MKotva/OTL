using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public float lifeTime;

    [Header("Scale Animation")]
    public float targetZScale = 0.1f;
    public float growDuration = 0.08f;
    public float shrinkDuration = 0.15f;

    [Header("End of life")]
    public float explosionLifetime;
    public bool destroyOnHit;
    public GameObject expirePrefab;
    public IOnHit onHit;

    [Header("Ignore")]
    public Transform ignoreRoot;
    public float ignoreRootDuration = 0.2f;
    private float age;
    private Vector3 originalScale;
    private bool exploded;
    public  Projectile selfPrefab;
    public DamageInstance DamageInstance {get;set;}

    void Start()
    {
        originalScale = transform.localScale;

     //   transform.localScale = new Vector3(
     //       originalScale.x,
     //       originalScale.y,
     //       0f
     //   );
         selfPrefab.DamageInstance = this.DamageInstance;
        selfPrefab.expirePrefab =this.expirePrefab;
        selfPrefab.explosionLifetime=this.explosionLifetime;
        selfPrefab.speed = this.speed;
        selfPrefab.lifeTime = this.lifeTime;
        selfPrefab.selfPrefab = this.selfPrefab;
      
    }
    
    void Update()
    {
        
        age += Time.deltaTime;

        transform.position += transform.forward * speed * Time.deltaTime;

       // UpdateScaleAnimation();

        if (age >= lifeTime)
            expire(transform.position);
    }

    void UpdateScaleAnimation()
    {
        float zScale = targetZScale;

        if (age < growDuration)
        {
            float t = age / growDuration;
            zScale = Mathf.Lerp(0f, targetZScale, t);
        }

        float timeUntilDeath = lifeTime - age;

        if (timeUntilDeath < shrinkDuration)
        {
            float t = timeUntilDeath / shrinkDuration;
            zScale = Mathf.Lerp(0f, targetZScale, t);
        }

        transform.localScale = new Vector3(
            originalScale.x,
            originalScale.y,
            zScale
        );
    }

    void OnTriggerEnter(Collider other)
    {

        if (ShouldIgnore(other))
            
            return;

        Vector3 hitPoint = other.ClosestPoint(transform.position);
        Debug.Log(onHit);
        onHit.HandleHit(this,other.gameObject, DamageInstance);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (ShouldIgnore(collision.collider))
            return;

        ContactPoint contact = collision.GetContact(0);
        onHit.HandleHit(this, collision.gameObject, DamageInstance);
    }

    bool ShouldIgnore(Collider other)
    {

        if (other == null)
            return true;

        if (ignoreRoot == null)
            return false;

        // Usually only needed right after firing, so the projectile can leave the ship.
        if (age > ignoreRootDuration)
            return false;

        if (other.transform == ignoreRoot)
            return true;

        if (other.transform.IsChildOf(ignoreRoot))
        {
            return true;
        }
        

        return false;
    }

    public void expire( Vector3 position)
    {
        if (exploded)
            return;

        exploded = true;

        if (expirePrefab != null)
        {
            GameObject explosion = Instantiate(
                expirePrefab,
                position,
                Quaternion.identity
            );

            if (explosionLifetime > 0f)
                Destroy(explosion, explosionLifetime);
        }

        Destroy(gameObject);
    }

    
}