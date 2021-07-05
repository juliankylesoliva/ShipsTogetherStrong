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

    /* ALLY SPAWN COOLDOWNS */
    private int currSpeedCD = 0;
    private int currRapidCD = 0;
    private int currMagnifyCD = 0;
    private int currScoreCD = 0;
    private int currShieldCD = 0;
    private int currReflectorCD = 0;
    private int currCopycatCD = 0;
    private int currBombCD = 0;
    private int currParasiteCD = 0;
    private int currLifeCD = 0;

    public int appliedSpeedCD = 2;
    public int appliedRapidCD = 2;
    public int appliedMagnifyCD = 2;
    public int appliedScoreCD = 3;
    public int appliedShieldCD = 5;
    public int appliedReflectorCD = 5;
    public int appliedCopycatCD = 10;
    public int appliedBombCD = 10;
    public int appliedParasiteCD = 20;
    public int appliedLifeCD = 100;

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
                tickDownCooldowns();

                int baseMaxEnemySpawns = minEnemySpawns + (playerShip.getTotalEnemiesDestroyed() / 100);

                if (baseMaxEnemySpawns > maxEnemySpawns)
                {
                    baseMaxEnemySpawns = maxEnemySpawns;
                }

                int numSpawns = Random.Range(minEnemySpawns, baseMaxEnemySpawns + 1);
                for (int i = 0; i < numSpawns; ++i)
                {
                    spawnLocation.rotation = Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), Vector3.forward);
                    spawnLocation.localPosition = (spawnLocation.up * spawnDistance);
                    GameObject objTemp = Instantiate(enemyPrefab, spawnLocation);
                    objTemp.transform.parent = null;

                    EnemyShipScript enemyShip = objTemp.GetComponent<EnemyShipScript>();
                    enemyShip.InitializeEnemy(this.gameObject.GetComponent<EnemySpawnerScript>());
                }
            }
        }
    }

    private void tickDownCooldowns()
    {
        if (currSpeedCD > 0) { --currSpeedCD; }
        if (currRapidCD > 0) { --currRapidCD; }
        if (currMagnifyCD > 0) { --currMagnifyCD; }
        if (currScoreCD > 0) { --currScoreCD; }
        if (currShieldCD > 0) { --currShieldCD; }
        if (currReflectorCD > 0) { --currReflectorCD; }
        if (currCopycatCD > 0) { --currCopycatCD; }
        if (currBombCD > 0) { --currBombCD; }
        if (currParasiteCD > 0) { --currParasiteCD; }
        if (currLifeCD > 0) { --currLifeCD; }
    }

    public void applyAllyCooldown(AllyType type)
    {
        switch (type)
        {
            case AllyType.Speed:
                currSpeedCD = appliedSpeedCD;
                break;
            case AllyType.Rapid:
                currRapidCD = appliedRapidCD;
                break;
            case AllyType.Magnify:
                currMagnifyCD = appliedMagnifyCD;
                break;
            case AllyType.Score:
                currScoreCD = appliedScoreCD;
                break;
            case AllyType.Shield:
                currShieldCD = appliedShieldCD;
                break;
            case AllyType.Reflector:
                currReflectorCD = appliedReflectorCD;
                break;
            case AllyType.Copycat:
                currCopycatCD = appliedCopycatCD;
                break;
            case AllyType.Bomb:
                currBombCD = appliedBombCD;
                break;
            case AllyType.Parasite:
                currParasiteCD = appliedParasiteCD;
                break;
            case AllyType.Life:
                currLifeCD = appliedLifeCD;
                break;
            default:
                break;
        }
    }

    public int getAllyCooldown(AllyType type)
    {
        switch (type)
        {
            case AllyType.Speed:
                return currSpeedCD;
            case AllyType.Rapid:
                return currRapidCD;
            case AllyType.Magnify:
                return currMagnifyCD;
            case AllyType.Score:
                return currScoreCD;
            case AllyType.Shield:
                return currShieldCD;
            case AllyType.Reflector:
                return currReflectorCD;
            case AllyType.Copycat:
                return currCopycatCD;
            case AllyType.Bomb:
                return currBombCD;
            case AllyType.Parasite:
                return currParasiteCD;
            case AllyType.Life:
                return currLifeCD;
            default:
                return 0;
        }
    }
}
