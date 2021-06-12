using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectShotScript : PlayerProjectile
{
    // Start is called before the first frame update
    void Start()
    {
        playerShip = transform.parent.parent.parent.parent.gameObject.GetComponent<MainShipMovement>();
        transform.parent = null;
        StartCoroutine(expireTimer());
    }

    // Checks for collisions
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.tag == "Enemy")
        {
            playerShip.increaseTotalEnemiesDestroyed();
            GameObject.Destroy(col.transform.gameObject);
            GameObject.Destroy(this.gameObject);
        }
        else if (col.transform.tag == "Ally" && (col.transform.parent == null || col.transform.parent.parent.tag == "Enemy"))
        {
            BaseAllyScript allyTemp = col.transform.gameObject.GetComponent<BaseAllyScript>();
            allyTemp.DestroyAllyShip();
            GameObject.Destroy(this.gameObject);
        }
        else { }
    }
}
