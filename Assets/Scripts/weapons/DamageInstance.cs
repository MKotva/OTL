using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class DamageInstance
    {
        public float DamageAmount {get;set;}
        public DamageSpread DamageSpread {get;}
        public DamageInstance(DamageSpread damageSpread, int damageAmount)
        {
        DamageSpread = damageSpread;
        DamageAmount = damageAmount;
        }
        
        public void ApplyDamage(GameObject hitObject)
    {
        Damageable damageable = hitObject.GetComponentInParent<Damageable>();

        if (damageable == null)
            return;

        damageable.TakeDamage(this);
    }
    }
