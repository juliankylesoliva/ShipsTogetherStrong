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
    private bool isGlideshot = false;
    private Vector3 initialPosition;
    private float shipVelocity;

    // Start is called before the first frame update
    void Start()
    {
        playerShip = transform.parent.parent.gameObject.GetComponent<MainShipMovement>();
        transform.parent = null;
        StartCoroutine(expireTimer());
        StartCoroutine(CheckGlideshot());
    }

    // Update is called once per frame
    void Update()
    {
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

            if (isGlideshot)
            {
                int baseVelocityScoring = (int)(shipVelocity * shipVelocity);
                int distanceMultiplier = (int)(this.transform.position - initialPosition).magnitude;
                if (baseVelocityScoring > 0 && distanceMultiplier > 0)
                {
                    playerShip.scoringSystem.AddToScore(baseVelocityScoring, distanceMultiplier, $"Glidesnipe (velocity {shipVelocity.ToString("n1")})");
                }
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

    // Player Projectile only: Check for Glideshot conditions
    private IEnumerator CheckGlideshot()
    {
        initialPosition = this.transform.position;
        shipVelocity = playerShip.getVelocity();
        isGlideshot = true;
        while (shipVelocity > 0.0f && !Input.GetKey(KeyCode.Space) && !playerShip.getIsDamaged() && !playerShip.getIsDamageproof())
        {
            yield return new WaitForSeconds(0.0f);
        }
        isGlideshot = false;
        yield break;
    }
}
