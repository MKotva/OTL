using System;
using UnityEngine;
[AttributeUsage(AttributeTargets.Field)]
public class DamageSpread : Attribute
{
    public float PhysicalDamage {get;set;}
    public float ShockDamage {get;set;}
    public float ThermalDamage {get;set;}
    public DamageSpread(float physicalDamage,float shockDamage,float thermalDamage )
    {
        this.PhysicalDamage = physicalDamage;
        this.ShockDamage = shockDamage;
        this.ThermalDamage = thermalDamage;
    }
}
