using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

   [SerializeField] private Transform frontGun1;
   [SerializeField] private Transform frontGun2;
   [SerializeField] private Transform dorsalUpperTurret1;
   [SerializeField] private Transform dorsalUpperTurret2;
   [SerializeField] private Transform dorsalLowerTurret3;
   [SerializeField] private Transform dorsalLowerTurret4;
   [SerializeField] private Transform pointLeftDefence1;
   [SerializeField] private Transform pointleftDefence2;
   [SerializeField] private Transform pointRightDefence3;
   [SerializeField] private Transform pointRightDefence4;
   public Dictionary<String,Transform> weaponLocations = new Dictionary<string, Transform>();
    void Awake()
    {
    weaponLocations.Add(nameof(frontGun1), frontGun1);
    weaponLocations.Add(nameof(frontGun2), frontGun2);
    weaponLocations.Add(nameof(dorsalUpperTurret1), dorsalUpperTurret1);
    weaponLocations.Add(nameof(dorsalUpperTurret2), dorsalUpperTurret2);
    weaponLocations.Add(nameof(dorsalLowerTurret3), dorsalLowerTurret3);
    weaponLocations.Add(nameof(dorsalLowerTurret4), dorsalLowerTurret4);
    weaponLocations.Add(nameof(pointLeftDefence1), pointLeftDefence1);
    weaponLocations.Add(nameof(pointleftDefence2), pointleftDefence2);
    weaponLocations.Add(nameof(pointRightDefence3), pointRightDefence3);
    weaponLocations.Add(nameof(pointRightDefence4), pointRightDefence4);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public GameObject AddWeapon(String name, GameObject weaponPrefab)
    {
         return Instantiate(
            weaponPrefab,
            weaponLocations[name].position,
            Quaternion.LookRotation(Vector3.left)
        );
    }
}
