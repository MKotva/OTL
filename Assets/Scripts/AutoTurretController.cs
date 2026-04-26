using System.Collections.Generic;
using UnityEngine;

public class AutoTurretController : MonoBehaviour
{
     [SerializeField]
    private Weapon weapon;
    private float fireTimer;
     [SerializeField]    private GameObject lastTarget;

    [SerializeField] private float range = 15f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private int maxColliders = 32;
    [SerializeField] private float scanInterval = 0.5f;
    
    private Collider[] hitsBuffer = new Collider[32];
    private float scanTimer;
    private GameObject currentTarget;
    [SerializeField]
    private float targetLock;
    private float targetTimer;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        scanTimer += Time.deltaTime;
        targetTimer -= Time.deltaTime;
        if (scanTimer >= scanInterval && targetTimer<= 0f)
        {
            scanTimer = 0f;
            
            currentTarget = FindClosestTarget();
            if(currentTarget != null)
            {
                targetTimer=targetLock;
            }
        }
        if(currentTarget != null)
        {
            
            Fire(currentTarget);
        }
    }
    void Fire(GameObject target)
    {

         fireTimer -= Time.deltaTime;
        while (fireTimer <= 0f){
            GameObject prepared = weapon.AimedFire(this.transform,target.transform);
            Projectile projectileController =
            prepared.GetComponent<Projectile>();
        if (projectileController != null)
        {
            projectileController = weapon.configureProjectile(projectileController);

            projectileController.ignoreRoot = transform.root;
        }
            fireTimer = weapon.fireRate;
        
    }
    }
    GameObject FindClosestTarget()
    {
        int hitCount = Physics.OverlapSphereNonAlloc(
            transform.position,
            range,
            hitsBuffer,
            enemyLayer
        );
        GameObject closest = null;
        float bestDist = float.MaxValue;
        Vector3 origin = transform.position;
        Debug.Log("Hit count "+ hitCount);
        for (int i = 0; i < hitCount; i++)
        {
            Collider col = hitsBuffer[i];
            if (col == null) continue;

            GameObject go = col.gameObject;

            float dist = (go.transform.position - origin).sqrMagnitude;

            if (dist < bestDist)
            {
                bestDist = dist;
                closest = go;
            }
        }
        lastTarget=closest;
        return closest;
    }
}
