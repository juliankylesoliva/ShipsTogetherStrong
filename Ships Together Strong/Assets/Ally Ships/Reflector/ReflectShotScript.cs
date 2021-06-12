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
}
