using UnityEngine;

[CreateAssetMenu(fileName = "CollisionHit", menuName = "Scriptable Objects/CollisionHit")]
public class CollisionHit : IOnHit
{
    public override void HandleHit(Projectile proj, GameObject hitObject, DamageInstance damInst)
    {
         if (damInst!= null)
            damInst.ApplyDamage(hitObject);
    }
}
