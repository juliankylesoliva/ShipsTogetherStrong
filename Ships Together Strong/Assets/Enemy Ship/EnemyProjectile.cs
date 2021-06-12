using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : PlayerProjectile
{
    void Start()
    {
        transform.parent = null;
        StartCoroutine(expireTimer());
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.tag == "Player")
        {
            MainShipMovement player = col.transform.gameObject.GetComponent<MainShipMovement>();
            player.TakeDamage();
            GameObject.Destroy(this.gameObject);
        }
        else if (col.transform.tag == "Ally" && (col.transform.parent != null && col.transform.parent.parent.tag == "Player"))
        {
            BaseAllyScript allyTemp = col.transform.gameObject.GetComponent<BaseAllyScript>();
            allyTemp.DestroyAllyShip(true);
            GameObject.Destroy(this.gameObject);
        }
        else { }
    }
}
