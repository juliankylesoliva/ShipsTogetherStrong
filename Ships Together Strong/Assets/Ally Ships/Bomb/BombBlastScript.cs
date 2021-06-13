using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBlastScript : PlayerProjectile
{
    private AudioSource soundPlayer;

    // Start is called before the first frame update
    void Start()
    {
        playerShip = GameObject.Find("Main Ship").GetComponent<MainShipMovement>();
        soundPlayer = this.gameObject.GetComponent<AudioSource>();
        transform.parent = null;
        soundPlayer.Play();
        StartCoroutine(expireTimer());
    }

    // Modified
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.tag == "Enemy")
        {
            playerShip.scoringSystem.AddToScore(baseShotScore, playerShip.getCurrentScoreMultiplier());

            playerShip.increaseTotalEnemiesDestroyed();
            GameObject.Destroy(col.transform.gameObject);
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
                playerShip.scoringSystem.AddToScore(baseShotScore, -5);
            }

            allyTemp.DestroyAllyShip();
        }
        else { }
    }
}
