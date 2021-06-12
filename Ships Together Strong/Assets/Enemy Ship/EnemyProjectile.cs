using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : PlayerProjectile
{
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.tag == "Player")
        {
            MainShipMovement player = col.transform.gameObject.GetComponent<MainShipMovement>();
            player.TakeDamage();
            GameObject.Destroy(this.gameObject);
        }
        else if (col.transform.tag == "Ally")
        {
            GameObject.Destroy(col.transform.gameObject);
            GameObject.Destroy(this.gameObject);
        }
        else { }
    }
}
