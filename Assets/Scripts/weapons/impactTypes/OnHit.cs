using UnityEngine;

public abstract class IOnHit : ScriptableObject
{
     public abstract void HandleHit(Projectile proj,GameObject hitObject,DamageInstance damInst );
}
