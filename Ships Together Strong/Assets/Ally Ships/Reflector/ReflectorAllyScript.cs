using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectorAllyScript : BaseAllyScript
{
    /* REFLECTOR SHIP VARIABLES */
    private int reflectState = 0; // 0 = Idle; 1 = Active; 2 = Holding
    public GameObject reflectShotPrefab;
    public Transform cannon;

    /* REFLECTOR SHIP METHODS */

    // Getter method for reflect state
    public int getReflectState()
    {
        return reflectState;
    }

    // Modified
    public override void DestroyAllyShip(bool isProjectile = false)
    {
        if (!isProjectile || reflectState == 0 || reflectState == 2)
        {
            GameObject.Destroy(this.gameObject);
        }
        else
        {
            reflectState = 2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttached && Input.GetMouseButton(0) && reflectState == 0)
        {
            reflectState = 1;
        }
        else if (isAttached && Input.GetMouseButton(0) && reflectState == 1)
        {
            reflectState = 1;
        }
        else if (isAttached && !Input.GetMouseButton(0) && reflectState == 1)
        {
            reflectState = 0;
        }
        else if (isAttached && Input.GetMouseButton(0) && reflectState == 2)
        {
            reflectState = 2;
        }
        else if (isAttached && !Input.GetMouseButton(0) && reflectState == 2)
        {
            reflectState = 0;
            Instantiate(reflectShotPrefab, cannon);
        }
        else
        {
            reflectState = 0;
        }
    }
}
