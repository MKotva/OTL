using UnityEngine;

public class EnemyGun : MonoBehaviour
{
[SerializeField]    public Weapon weapon;

[SerializeField]     public EnemyMovementScript enemyMovementScript;
[SerializeField] float targetingRange;

[SerializeField] public GameObject mainTarget;
private float fireTimer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //enemyMovementScript.mainTarget = this.mainTarget;
    }
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
         fireTimer -= Time.deltaTime;
         Debug.Log(enemyMovementScript.mainTarget);
         Debug.Log(mainTarget);
        while (enemyMovementScript.mainTarget !=null && Vector3.Distance(enemyMovementScript.mainTarget.transform.position, this.transform.position)<=targetingRange&& fireTimer <= 0f){
            GameObject prepared = weapon.AimedFire(this.transform,mainTarget.transform);
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
}
