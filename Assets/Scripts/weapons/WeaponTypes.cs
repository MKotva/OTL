using System.Collections.Generic;
using UnityEngine;

public enum WeaponTypes
{
   laser ,
   plasma,
   physical 
}

public static class WeaponTypesDict
{    static WeaponTypesDict()
    {
        Dictionary<WeaponTypes,DamageSpread> damageSpreads = new Dictionary<WeaponTypes, DamageSpread>();
        damageSpreads.Add(WeaponTypes.laser,new DamageSpread(0.33f,0.33f,0.33f));
        damageSpreads.Add(WeaponTypes.plasma,new DamageSpread(0.33f,0.33f,0.33f));
        damageSpreads.Add(WeaponTypes.physical,new DamageSpread(0.33f,0.33f,0.33f));
        DamageSpreads = damageSpreads;
    }
    public static Dictionary<WeaponTypes,DamageSpread> DamageSpreads {get;private set;}

}