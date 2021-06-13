using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    /* PROJECTILE VARIABLES */
    public float projectileSpeed = 10.0f;
    public float activeTime = 1.0f;
    public int baseShotScore = 10;

    /* PRIVATE VARIABLES */
    [HideInInspector] public MainShipMovement playerShip;

    // Start is called before the first frame update
    void Start()
    {
        playerShip = transform.parent.parent.gameObject.GetComponent<MainShipMovement>();
        transform.parent = null;
        StartCoroutine(expireTimer());
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position += (this.transform.up * Time.deltaTime * projectileSpeed);
    }

    // Projectile despawns after a short period of time
    public IEnumerator expireTimer()
    {
        yield return new WaitForSeconds(activeTime);
        GameObject.Destroy(this.gameObject);
    }

    // Checks for collisions
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.tag == "Enemy")
        {
            playerShip.scoringSystem.AddToScore(baseShotScore, playerShip.getCurrentScoreMultiplier());

            playerShip.increaseTotalEnemiesDestroyed();
            GameObject.Destroy(col.transform.gameObject);
            GameObject.Destroy(this.gameObject);
        }
        else if (col.transform.tag == "Ally" && (col.transform.parent == null || col.transform.parent.parent.tag == "Enemy"))
        {
            BaseAllyScript allyTemp = col.transform.gameObject.GetComponent<BaseAllyScript>();

            if (allyTemp.getPowerupType() == AllyType.Parasite)
            {
                playerShip.scoringSystem.AddToScore(baseShotScore, playerShip.getCurrentScoreMultiplier() * 10);
            }
            else
            {
                playerShip.scoringSystem.AddToScore(baseShotScore, -10);
            }

            allyTemp.DestroyAllyShip();
            GameObject.Destroy(this.gameObject);
        }
        else { }
    }
}
