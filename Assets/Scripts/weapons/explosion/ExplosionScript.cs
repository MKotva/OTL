using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Explosion : MonoBehaviour
{
     public DamageInstance damageInstance {get;set;}
    [SerializeField] public float radius;
     private List<Damageable> hitObject;
     private List<Collider> hitColliders;
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GetCollidersInSphere(Vector3 center, float rad, LayerMask layerMask)
    {
         hitColliders = Physics.OverlapSphere(center, radius, layerMask).ToList();
        
    }
}
