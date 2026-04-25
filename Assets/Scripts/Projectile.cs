using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 40f;
    public float lifeTime = 4f;

    [Header("Scale Animation")]
    public float targetZScale = 0.1f;
    public float growDuration = 0.08f;
    public float shrinkDuration = 0.15f;

    [Header("Impact")]
    public GameObject explosionPrefab;
    public float explosionLifetime = 2f;
    public bool destroyOnHit = true;

    [Header("Ignore")]
    public Transform ignoreRoot;
    public float ignoreRootDuration = 0.2f;

    private float age;
    private Vector3 originalScale;
    private bool exploded;

    void Start()
    {
        originalScale = transform.localScale;

        transform.localScale = new Vector3(
            originalScale.x,
            originalScale.y,
            0f
        );
    }

    void Update()
    {
        age += Time.deltaTime;

        transform.position += transform.forward * speed * Time.deltaTime;

        UpdateScaleAnimation();

        if (age >= lifeTime)
            ExplodeAndDestroy();
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
        HandleHit(other.gameObject, hitPoint);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (ShouldIgnore(collision.collider))
            return;

        ContactPoint contact = collision.GetContact(0);
        HandleHit(collision.gameObject, contact.point);
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
            return true;

        return false;
    }

    void HandleHit(GameObject hitObject, Vector3 hitPoint)
    {
        PhysicalDamageSource physicalDamage = GetComponent<PhysicalDamageSource>();

        if (physicalDamage != null)
            physicalDamage.ApplyDamage(hitObject);

        if (destroyOnHit)
            ExplodeAndDestroy(hitPoint);
    }

    public void ExplodeAndDestroy()
    {
        ExplodeAndDestroy(transform.position);
    }

    public void ExplodeAndDestroy(Vector3 position)
    {
        if (exploded)
            return;

        exploded = true;

        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(
                explosionPrefab,
                position,
                Quaternion.identity
            );

            if (explosionLifetime > 0f)
                Destroy(explosion, explosionLifetime);
        }

        Destroy(gameObject);
    }
}