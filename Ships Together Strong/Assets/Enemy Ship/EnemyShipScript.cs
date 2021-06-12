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
    public float spawnDistance = 18.0f;
    public float despawnDistance = 50.0f;

    /* PRIVATE ENEMY VARIABLES */
    private GameObject playerShip;
    private float distanceToPlayer = -1.0f;

    /* PREFABS AND OTHER DRAG AND DROPS */
    public Transform cannon;
    public GameObject projectile;
    public Transform captureSlot;
    public GameObject[] allySpawnList;

    // Start is called before the first frame update
    void Start()
    {
        playerShip = GameObject.Find("Main Ship");
        rb2D = this.gameObject.GetComponent<Rigidbody2D>();
        StartCoroutine(calcDistanceToPlayer());
        StartCoroutine(EnemyMove());
        StartCoroutine(EnemyFire());
        StartCoroutine(EnemyRotate());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Moves the enemy ship in the direction it's facing
    IEnumerator EnemyMove()
    {
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
            if (isInRange())
            {
                yield return new WaitForSeconds(enemyFiringInterval);
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
            if (isInRange())
            {
                Vector3 dirVec = playerShip.transform.position - this.transform.position;
                float angle = (Mathf.Atan2(dirVec.y, dirVec.x) * Mathf.Rad2Deg) - 90.0f;
                this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            yield return new WaitForSeconds(0.01f);
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
    private bool isInRange()
    {
        return distanceToPlayer > 0.0f && distanceToPlayer <= enemyChaseDistance;
    }
}
