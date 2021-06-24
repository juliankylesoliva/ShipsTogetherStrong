using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    /* COMPONENTS */
    private Rigidbody2D rb2D;

    /* PROJECTILE VARIABLES */
    public string projectileName = "Player";
    public float projectileSpeed = 10.0f;
    public float activeTime = 1.0f;
    public int baseShotScore = 10;
    public int basePenaltyScore = 5;
    public int enemyRangeScoreMultiplier = 2;

    /* PRIVATE VARIABLES */
    [HideInInspector] public MainShipMovement playerShip;

    // Start is called before the first frame update
    void Start()
    {
        rb2D = this.gameObject.GetComponent<Rigidbody2D>();
        playerShip = transform.parent.parent.gameObject.GetComponent<MainShipMovement>();
        transform.parent = null;
        StartCoroutine(expireTimer());
    }

    // Update is called once per frame
    void Update()
    {
        //rb2D.AddForce(this.transform.up * projectileSpeed, ForceMode2D.Impulse);
        this.transform.position += (this.transform.up * Time.deltaTime * projectileSpeed);
    }

    // Projectile despawns after a short period of time
    protected IEnumerator expireTimer()
    {
        yield return new WaitForSeconds(activeTime);
        GameObject.Destroy(this.gameObject);
    }

    // Checks for collisions
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.tag == "Enemy")
        {
            EnemyShipScript enemyTemp = col.transform.gameObject.GetComponent<EnemyShipScript>();

            if (enemyTemp.isInRange())
            {
                playerShip.scoringSystem.AddToScore(baseShotScore, playerShip.getCurrentScoreMultiplier() * enemyRangeScoreMultiplier, $"Hit Nearby Enemy ({projectileName})");
            }
            else
            {
                playerShip.scoringSystem.AddToScore(baseShotScore, playerShip.getCurrentScoreMultiplier(), $"Hit Enemy ({projectileName})");
            }

            playerShip.increaseTotalEnemiesDestroyed();
            GameObject.Destroy(col.transform.gameObject);
            GameObject.Destroy(this.gameObject);
        }
        else if (col.transform.tag == "Ally" && (col.transform.parent == null || col.transform.parent.parent.tag == "Enemy"))
        {
            BaseAllyScript allyTemp = col.transform.gameObject.GetComponent<BaseAllyScript>();

            if (allyTemp.getPowerupType() == AllyType.Parasite)
            {
                playerShip.scoringSystem.AddToScore(baseShotScore, playerShip.getCurrentScoreMultiplier() * 10, $"Hit Parasite ({projectileName})");
            }
            else
            {
                playerShip.scoringSystem.AddToScore(basePenaltyScore, -1, $"Friendly Fire ({projectileName})");
            }

            allyTemp.DestroyAllyShip();
            GameObject.Destroy(this.gameObject);
        }
        else { }
    }
}
