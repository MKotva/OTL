using UnityEngine;

using UnityEngine;

public class PhysicalDamageSource : MonoBehaviour
{
    public float physicalDamage = 10f;

    public void ApplyDamage(GameObject hitObject)
    {
        Damageable damageable = hitObject.GetComponentInParent<Damageable>();

        if (damageable == null)
            return;

        damageable.TakeDamage(physicalDamage);
    }
}
