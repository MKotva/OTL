using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class EnemyOrchestrator : MonoBehaviour
{
    [SerializeField]
    private ScoreBoard scoreBoard;
    private int round;
    private List<int> currentActive = new List<int>();
    [SerializeField]
    private GameObject basicEnemyPrefab;
    [SerializeField] public GameObject mainTarget;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentActive.Count <= 0)
        {
            createNewRound();
        }
    }
    private void createNewRound()
    {
        for(int i =0; i < 10; i += 1)
        {
            GameObject enemy = Instantiate(
            this.basicEnemyPrefab,
            this.transform.position+ new Vector3(round,round,round),
            this.transform.rotation
        );

       
         DamagableEnemy damg =enemy.GetComponent<DamagableEnemy>();
         EnemyMovementScript move =enemy.GetComponent<EnemyMovementScript>();
          EnemyGun egun = enemy.GetComponent<EnemyGun>();
          move.mainTarget = mainTarget;
         egun.mainTarget = mainTarget;
         egun.enemyMovementScript=move;
         damg.ident = i;
         damg.master = this;
         currentActive.Add(i);
         
        }
    }
    public void deregister(int ident)
    {
        currentActive.Remove(ident);
    }
}
