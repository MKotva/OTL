using UnityEngine;

public class BeamWeapon : Weapon
{[SerializeField]    public float beamReach;

     public override GameObject Fire(Transform firePoint )
    {   float targetDistance = beamReach;
        
        RaycastHit coll;
        Physics.Raycast(firePoint.position, firePoint.forward, out coll, beamReach);
        if (coll.collider != null)
        {
            targetDistance = coll.distance;
        }   
    
        GameObject beam = Instantiate(
            this.projectilePrefab,
            firePoint.position,
            firePoint.rotation
        );
        BoxCollider colBox = beam.GetComponent<BoxCollider>();
         float currentLength = colBox.size.z;
        
            float scaleFactor = targetDistance / currentLength;
            beam.transform.localScale = new Vector3(beam.transform.localScale.x,beam.transform.localScale.y,beam.transform.localScale.z*(scaleFactor*1.1f));
            float updatedLength = beam.GetComponent<BoxCollider>().size.z;
            beam.transform.position = firePoint.position + firePoint.forward * (updatedLength*scaleFactor / 10f);
            Physics.SyncTransforms();
         return beam;
    }
}
