using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBlastScript : PlayerProjectile
{
    // Start is called before the first frame update
    void Start()
    {
        playerShip = GameObject.Find("Main Ship").GetComponent<MainShipMovement>();
        transform.parent = null;
        StartCoroutine(expireTimer());
    }

    // Modified
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.tag == "Enemy")
        {
            playerShip.increaseTotalEnemiesDestroyed();
            GameObject.Destroy(col.transform.gameObject);
        }
        else if (col.transform.tag == "Ally" && (col.transform.parent == null || col.transform.parent.parent.tag == "Enemy"))
        {
            BaseAllyScript allyTemp = col.transform.gameObject.GetComponent<BaseAllyScript>();
            allyTemp.DestroyAllyShip();
        }
        else { }
    }
}
