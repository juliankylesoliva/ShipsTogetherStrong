using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopycatAllyScript : BaseAllyScript
{
    /* COPYCAT SHIP VARIABLES */
    public GameObject copyShotPrefab;
    public Transform cannon;
    public float copyFiringDelay = 0.85f;
    private bool isFiringDelayed = false;

    // Update is called once per frame
    void Update()
    {
        if (isAttached && !isFiringDelayed && Input.GetMouseButton(0))
        {
            Instantiate(copyShotPrefab, cannon);
            StartCoroutine(DoFiringDelay());
        }
    }

    // Helper function for making a firing delay
    IEnumerator DoFiringDelay()
    {
        isFiringDelayed = true;
        yield return new WaitForSeconds(copyFiringDelay);
        isFiringDelayed = false;
    }
}
