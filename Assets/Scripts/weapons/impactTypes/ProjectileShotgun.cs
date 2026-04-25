using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
[CreateAssetMenu(fileName = "ProjectileShotgun", menuName = "Hit Handlers/ProjectileShotgun")]
public class ProjectileShotgun :  IOnHit
{
    public GameObject explosionPrefab;
    public float explosionLifetime =2f;
    private bool exploded {get;set;}
    private IOnHit ChildHit;
     public  override void HandleHit(Projectile proj,GameObject hitObject,DamageInstance damInst)
    {
        
        if (damInst!= null)
            damInst.ApplyDamage(hitObject);
            
            ExplodeAndDestroy(proj);
    }
    

    public void ExplodeAndDestroy(Projectile proj)
    {
        Debug.Log("Shotgun blas");
        ShotgunBlast(proj);
        if (exploded)
            return;

        exploded = true;
        
        
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(
                explosionPrefab,
                proj.gameObject.transform.position,
                Quaternion.identity
            );

            if (explosionLifetime > 0f)
                Destroy(explosion, explosionLifetime);
        }

        Destroy(proj);
    }
    private void ShotgunBlast(Projectile proj)
    {
        Debug.Log("Shotgun blast");
        List<Vector3> directions = new List<Vector3>
        {
        // Cardinal directions (6)
            Vector3.right,
            Vector3.left,
            Vector3.forward,
            Vector3.back,
            Vector3.up,
            Vector3.down,

            // Cube corners (8)
            new Vector3(1, 1, 1),
            new Vector3(1, 1, -1),
            new Vector3(1, -1, 1),
            new Vector3(1, -1, -1),
            new Vector3(-1, 1, 1),
            new Vector3(-1, 1, -1),
            new Vector3(-1, -1, 1),
            new Vector3(-1, -1, -1),

            // Extra “up/down variants” (2) — adjust as needed
            new Vector3(0.5f, 1f, 0f),
            new Vector3(0.5f, -1f, 0f),
        };
        float spawnRadius = 1f;
        foreach (var dir in directions)
        {
            Vector3 direction = dir.normalized;
            Vector3 spawnPosition = proj.transform.position + direction * spawnRadius;
             Vector3 normalized = dir.normalized;

            Quaternion rotation = Quaternion.LookRotation(normalized);
            Debug.Log(proj.selfPrefab.lifeTime);
            Debug.Log(proj.selfPrefab.onHit);
            Projectile poj = Instantiate(proj.selfPrefab, spawnPosition, rotation);
            
        }
    }
}
