using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerScript : MonoBehaviour
{
    /* ENABLES */
    public bool enableSpawning = true;

    /* COMPONENTS */
    MainShipMovement playerShip;

    /* DRAG AND DROPS */
    public GameObject enemyPrefab;
    public Transform spawnLocation;

    /* Spawner Variables */
    public float spawnDistance = 18.0f;
    public float baseSpawnInterval = 3.0f;
    public float spawnIntervalDeviation = 1.0f;
    public int minEnemySpawns = 1;
    public int maxEnemySpawns = 3;

    // Start is called before the first frame update
    void Start()
    {
        playerShip = this.transform.parent.gameObject.GetComponent<MainShipMovement>();
        StartCoroutine(SpawnerCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnerCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(baseSpawnInterval + Random.Range(-spawnIntervalDeviation, spawnIntervalDeviation));
            if (enableSpawning)
            {
                int baseMaxEnemySpawns = minEnemySpawns + (playerShip.getTotalEnemiesDestroyed() / 100);

                if (baseMaxEnemySpawns > maxEnemySpawns)
                {
                    baseMaxEnemySpawns = maxEnemySpawns;
                }

                int numSpawns = Random.Range(minEnemySpawns, baseMaxEnemySpawns);
                for (int i = 0; i < numSpawns; ++i)
                {
                    spawnLocation.rotation = Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), Vector3.forward);
                    spawnLocation.localPosition = (spawnLocation.up * spawnDistance);
                    GameObject objTemp = Instantiate(enemyPrefab, spawnLocation);
                    objTemp.transform.parent = null;

                    EnemyShipScript enemyShip = objTemp.GetComponent<EnemyShipScript>();
                    enemyShip.InitializeEnemy();
                }
            }
        }
    }
}
