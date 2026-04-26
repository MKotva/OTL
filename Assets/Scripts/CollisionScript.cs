using UnityEngine;

public class CollisionScript : MonoBehaviour
{
    [SerializeField] private Collider collider;
    [SerializeField] private IOnHit onHit;
    public Transform ignoreRoot;

  
    public DamageInstance DamageInstance {get;set;}
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DamageInstance = new DamageInstance(WeaponTypesDict.DamageSpreads[WeaponTypes.physical],100);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        Rigidbody rig =  other.gameObject.GetComponent<Rigidbody>();
        if (rig != null)
        {
           // DamageInstance.DamageAmount = rig.linearVelocity.magnitude;
        }
        if (ShouldIgnore(other))
        {
            return;
        }
        onHit.HandleHit(null,other.gameObject, DamageInstance);
    }

    void OnCollisionEnter(Collision other)
    {
        Rigidbody rig =  other.gameObject.GetComponent<Rigidbody>();
        if (rig != null)
        {
           // DamageInstance.DamageAmount = rig.linearVelocity.magnitude;
        }
     if (ShouldIgnore(other.collider))
        {
            return;
        }
        onHit.HandleHit(null,other.gameObject, DamageInstance);
    }

    bool ShouldIgnore(Collider other)
    {

        if (other == null)
            return true;

        if (ignoreRoot == null)
            return false;

        if (other.transform == ignoreRoot)
            return true;

        if (other.transform.IsChildOf(ignoreRoot))
        {
            return true;
        }
        

        return false;
    }
}
