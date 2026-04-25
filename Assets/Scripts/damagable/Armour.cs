using UnityEngine;

public class Armour
{
    public int ShockResistance {get;}
    public int ThermalResistance {get;}
    public int PhysicalResistance {get;}
     public Armour(int shockResistance, int heatResistance, int physicalResistance)
    {
        ShockResistance = shockResistance;
        ThermalResistance = heatResistance;
        PhysicalResistance = physicalResistance;
    }
    
   
    public Armour() : this(0, 0, 0)
    {
    }
}
