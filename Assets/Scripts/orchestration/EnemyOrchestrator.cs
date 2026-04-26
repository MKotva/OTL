using System.Collections.Generic;
using UnityEngine;

public class EnemyOrchestrator : MonoBehaviour
{
    [SerializeField]
    private ScoreBoard scoreBoard;

    [SerializeField]
    private int enemyCount = 3;
    private int round;
    private List<int> currentActive = new List<int>();

    [SerializeField]
    private GameObject basicEnemyPrefab;
    [SerializeField]
    public GameObject mainTarget;

    private bool spawningEnabled;

    void Update()
    {
        if (!spawningEnabled)
            return;

        if (currentActive.Count <= 0)
        {
            CreateNewRound();
        }
    }

    public void SetSpawningEnabled(bool enabled)
    {
        spawningEnabled = enabled;
    }

    private void CreateNewRound()
    {
        round++;

        currentActive.Clear();

        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 spawnOffset = new Vector3(
                i * 4f,
                0f,
                round * 8f
            );

            GameObject enemy = Instantiate(
                basicEnemyPrefab,
                transform.position + spawnOffset,
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
    }

    public void deregister(int ident)
    {
        currentActive.Remove(ident);
    }
}