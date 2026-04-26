using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.AI;

public class EnemyOrchestrator : MonoBehaviour
{
    [SerializeField]
    private ScoreBoard scoreBoard;
    [SerializeField]
    private SpaceBoundaries spaceBoundaries;

    [SerializeField]
    private int enemyCount = 3;
    [SerializeField]
    private int enemyRoundIncrease = 15;

    private int round;
    private List<int> currentActive = new List<int>();

    [SerializeField]
    private List<GameObject> enemyPrefab;

    [SerializeField]
    public GameObject mainTarget;

    [Header("Round Rewards")]
    [SerializeField]
    private WeaponManager weaponUnlockManager;

    [SerializeField]
    private float delayBeforeNextRound = 2f;
  
    private bool spawningEnabled;
    private bool roundTransitionRunning;

    void Update()
    {
        if (!spawningEnabled)
            return;

        if (roundTransitionRunning)
            return;

        if (currentActive.Count <= 0)
        {
            if (round <= 0)
                CreateNewRound();
            else
                StartCoroutine(HandleRoundCompleted());
        }
    }

    public void SetSpawningEnabled(bool enabled)
    {
        spawningEnabled = enabled;
    }

    private IEnumerator HandleRoundCompleted()
    {
        roundTransitionRunning = true;

        if (weaponUnlockManager != null)
            weaponUnlockManager.UnlockRandomWeapon();

        yield return new WaitForSeconds(delayBeforeNextRound);

        CreateNewRound();

        roundTransitionRunning = false;
    }

    private void CreateNewRound()
    {
        Debug.Log("Round " + round);
        round++;
        enemyCount += enemyRoundIncrease;
        int groups = Math.Min((int)enemyCount/10,10);
        int enemyPerGroup = enemyCount/groups;
        for(int i = 0; i <= groups; i++)
        {
             for(int it = 0; it <= enemyPerGroup; it++)
        {
            int randomUnit = UnityEngine.Random.Range(0, enemyPrefab.Count);
            createNewEnemy(enemyPrefab[randomUnit],createGroupCenter(0, (int)spaceBoundaries.innerRadius, mainTarget,it),it);
        }
        }
        for (int i = 0; i < enemyCount; i++)
        {
            UnityEngine.Vector3 spawnOffset = new UnityEngine.Vector3(
                i * 4f,
                0f,
                round * 8f
            );

            
        }
    }

    public void deregister(int ident)
    {
        currentActive.Remove(ident);
        scoreBoard.increaseScore(1);
    }
    private void createNewEnemy(GameObject enemyPrefab, UnityEngine.Vector3 startingPosition, int i)
    {
        GameObject enemy = Instantiate(
                enemyPrefab,
                startingPosition,
                transform.rotation
            );

            DamagableEnemy damg = enemy.GetComponent<DamagableEnemy>();
            EnemyMovementScript move = enemy.GetComponent<EnemyMovementScript>();
            EnemyGun egun = enemy.GetComponent<EnemyGun>();

            if (move != null)
                move.mainTarget = mainTarget;

            if (egun != null)
            {
                egun.mainTarget = mainTarget;
                egun.enemyMovementScript = move;
            }

            if (damg != null)
            {
                damg.ident = i;
                damg.master = this;
            }

            currentActive.Add(i);
    }
    private UnityEngine.Vector3 createGroupCenter(int min, int max,GameObject player, int increment)
    {
        int x = UnityEngine.Random.Range(min, max);
        int y = UnityEngine.Random.Range(min, max);
        int z = UnityEngine.Random.Range(min, max);
        UnityEngine.Vector3 temp = new UnityEngine.Vector3(x+increment,y+increment,y+increment);
        int maxCount = 0;
        while(maxCount <= 10 && UnityEngine.Vector3.Distance(player.transform.position, temp) <= 150){
            x = UnityEngine.Random.Range(min, max);
            y = UnityEngine.Random.Range(min, max);
            z = UnityEngine.Random.Range(min, max);
            temp = new UnityEngine.Vector3(x+increment,y+increment,y+increment);
            maxCount += 1;
        }
        return  temp;
    }
}