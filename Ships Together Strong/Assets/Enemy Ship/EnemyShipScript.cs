using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipScript : MonoBehaviour
{
    /* COMPONENTS */
    private Rigidbody2D rb2D;

    /* ENEMY VARIABLES */
    public float enemySpeed = 1.5f;
    public float enemyFiringInterval = 1.0f;
    public float enemyChaseDistance = 4.5f;
    public float despawnDistance = 50.0f;
    public float spawnAngleDeviation = 5.0f;

    /* PRIVATE ENEMY VARIABLES */
    private GameObject playerShip;
    private float distanceToPlayer = -1.0f;
    private BaseAllyScript capturedAlly = null;
    private MainShipMovement playerScript;
    private AudioSource soundPlayer;

    /* PREFABS AND OTHER DRAG AND DROPS */
    public Transform cannon;
    public GameObject projectile;
    public Transform captureSlot;
    public GameObject[] allyPrefabs;
    public AudioClip[] soundEffects;
    public GameObject explosionPrefab;

    // Start is called before the first frame update
    void Start()
    {
        playerShip = GameObject.Find("Main Ship");
        playerScript = playerShip.GetComponent<MainShipMovement>();
        rb2D = this.gameObject.GetComponent<Rigidbody2D>();
        soundPlayer = this.gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (distanceToPlayer >= despawnDistance)
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    // Initializes the enemy's AI
    public void InitializeEnemy()
    {
        if (playerShip == null)
        {
            playerShip = GameObject.Find("Main Ship");
        }

        if (playerScript == null)
        {
            playerScript = playerShip.GetComponent<MainShipMovement>();
        }

        pointTowardsPlayer(spawnAngleDeviation);
        rollForCapturedAlly(50 + playerScript.getTotalEnemiesDestroyed());
        StartCoroutine(calcDistanceToPlayer());
        StartCoroutine(EnemyMove());
        StartCoroutine(EnemyFire());
        StartCoroutine(EnemyRotate());
    }

    // Moves the enemy ship in the direction it's facing
    IEnumerator EnemyMove()
    {
        if (rb2D == null)
        {
            rb2D = this.gameObject.GetComponent<Rigidbody2D>();
        }

        while (true)
        {
            rb2D.AddForce(this.transform.up * enemySpeed);
            yield return new WaitForSeconds(0.01f);
        }
    }

    // Enemy fires at a constant interval
    IEnumerator EnemyFire()
    {
        while(true)
        {
            if (isInRange() && !playerScript.getIsDamaged())
            {
                yield return new WaitForSeconds(enemyFiringInterval);
                PlaySoundEffect(soundEffects[0]);
                Instantiate(projectile, cannon);
            }
            yield return new WaitForSeconds(0.01f);
        }
    }

    // Enemy gradually rotates toward the player
    IEnumerator EnemyRotate()
    {
        while (true)
        {
            if (isInRange() && !playerScript.getIsDamaged())
            {
                pointTowardsPlayer();
            }
            else if (playerScript.getIsDamaged())
            {
                pointTowardsPlayer(0.0f, true);
            }
            else { }

            yield return new WaitForSeconds(0.01f);
        }
    }

    // Points toward the player
    void pointTowardsPlayer(float randomizer = 0.0f, bool opposite = false)
    {
        if (playerShip == null)
        {
            playerShip = GameObject.Find("Main Ship");
        }

        Vector3 dirVec = playerShip.transform.position - this.transform.position;
        float angle = (Mathf.Atan2(dirVec.y, dirVec.x) * Mathf.Rad2Deg) - 90.0f;

        if (opposite)
        {
            this.transform.rotation = Quaternion.AngleAxis(angle + (Random.Range(-randomizer, randomizer)) + 180.0f, Vector3.forward);
        }
        else
        {
            this.transform.rotation = Quaternion.AngleAxis(angle + (Random.Range(-randomizer, randomizer)), Vector3.forward);
        }
    }

    // Helper function that constantly calculates distance to player
    IEnumerator calcDistanceToPlayer()
    {
        while (true)
        {
            distanceToPlayer = Vector3.Distance(playerShip.transform.position, this.transform.position);
            yield return new WaitForSeconds(0.01f);
        }
    }

    // Helper function that checks if the player is within firing range
    public bool isInRange()
    {
        return distanceToPlayer > 0.0f && distanceToPlayer <= enemyChaseDistance;
    }

    // Handles collision cases
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.tag == "Player")
        {
            Explode();
            MainShipMovement player = col.transform.gameObject.GetComponent<MainShipMovement>();
            player.TakeDamage();

            if (capturedAlly != null)
            {
                capturedAlly.DetachFromShip();
            }

            GameObject.Destroy(this.gameObject);
        }
        else if (col.transform.tag == "Ally" && (col.transform.parent != null && col.transform.parent.parent.tag == "Player"))
        {
            Explode();
            BaseAllyScript allyTemp = col.transform.gameObject.GetComponent<BaseAllyScript>();
            allyTemp.DestroyAllyShip();

            if (capturedAlly != null)
            {
                capturedAlly.DetachFromShip();
            }

            GameObject.Destroy(this.gameObject);
        }
        else if (col.transform.tag == "PlayerAttack")
        {
            Explode();
            if (capturedAlly != null)
            {
                capturedAlly.DetachFromShip(0.0f, true);
            }
        }
        else { }
    }

    // Random chance to spawn a captured ally
    void rollForCapturedAlly(int limit = 50)
    {
        int upper = limit;
        if (upper > 100)
        {
            upper = 100;
        }

        int rng = Random.Range(1, (upper + 1));

        if (rng >= 1 && rng <= 10)
        {
            // No ally
        }
        else if (rng >= 11 && rng <= 30)
        {
            spawnCapturedAlly(AllyType.Speed);
        }
        else if (rng >= 31 && rng <= 50)
        {
            spawnCapturedAlly(AllyType.Rapid);
        }
        else if (rng >= 51 && rng <= 70)
        {
            spawnCapturedAlly(AllyType.Magnify);
        }
        else if (rng >= 71 && rng <= 75)
        {
            spawnCapturedAlly(AllyType.Shield);
        }
        else if (rng >= 76 && rng <= 80)
        {
            spawnCapturedAlly(AllyType.Reflector);
        }
        else if (rng >= 81 && rng <= 85)
        {
            spawnCapturedAlly(AllyType.Copycat);
        }
        else if (rng >= 86 && rng <= 90)
        {
            spawnCapturedAlly(AllyType.Score);
        }
        else if (rng >= 91 && rng <= 95)
        {
            spawnCapturedAlly(AllyType.Bomb);
        }
        else if (rng >= 96 && rng <= 98)
        {
            spawnCapturedAlly(AllyType.Parasite);
        }
        else if (rng >= 99 && rng <= 100)
        {
            spawnCapturedAlly(AllyType.Life);
        }
        else { /* Nothing */ }
    }

    // Spawn a captured ally
    void spawnCapturedAlly(AllyType type = AllyType.Base)
    {
        GameObject objTemp;

        switch (type)
        {
            case AllyType.Speed:
                objTemp = Instantiate(allyPrefabs[0], captureSlot);
                break;
            case AllyType.Rapid:
                objTemp = Instantiate(allyPrefabs[0], captureSlot);
                break;
            case AllyType.Magnify:
                objTemp = Instantiate(allyPrefabs[0], captureSlot);
                break;
            case AllyType.Score:
                objTemp = Instantiate(allyPrefabs[0], captureSlot);
                break;
            case AllyType.Shield:
                objTemp = Instantiate(allyPrefabs[1], captureSlot);
                break;
            case AllyType.Reflector:
                objTemp = Instantiate(allyPrefabs[2], captureSlot);
                break;
            case AllyType.Copycat:
                objTemp = Instantiate(allyPrefabs[3], captureSlot);
                break;
            case AllyType.Bomb:
                objTemp = Instantiate(allyPrefabs[4], captureSlot);
                break;
            case AllyType.Parasite:
                objTemp = Instantiate(allyPrefabs[5], captureSlot);
                break;
            case AllyType.Life:
                objTemp = Instantiate(allyPrefabs[6], captureSlot);
                break;
            default:
                objTemp = Instantiate(allyPrefabs[0], captureSlot);
                break;
        }

        capturedAlly = objTemp.GetComponent<BaseAllyScript>();
        capturedAlly.setPowerupType(type);
        capturedAlly.AttachToEnemy(captureSlot);
    }

    // Plays sound effects
    private void PlaySoundEffect(AudioClip theClip)
    {
        soundPlayer.clip = theClip;
        soundPlayer.Play();
    }

    public void Explode()
    {
        GameObject objTemp = Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
        ExplodeSoundScript explosionScript = objTemp.GetComponent<ExplodeSoundScript>();
        explosionScript.DoExplosion(1, 0.5f);
    }
}
