using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainShipMovement : MonoBehaviour
{
    /* COMPONENTS */
    private Camera mainCam;
    private Rigidbody2D rb2D;

    /* ENABLES */
    public bool enableMouseMovement = true;
    public bool enableShipMovement = true;
    public bool enableShooting = true;

    /* SHIP VARIABLES */
    public float baseShipSpeed = 1.0f;
    public float baseFiringDelay = 0.5f;

    /* PRIVATE SHIP VARIABLES */
    private bool isFiringDelayed = false;

    /* SHIP PREFABS AND OTHER DRAG AND DROPS */
    public Transform cannon;
    public GameObject projectile;
    public Transform[] formationSlots;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        mainCam = Camera.main;
        rb2D = this.gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enableMouseMovement) { PointToMouse(); }
        if (enableShipMovement) { MoveShip(); }
        if (enableShooting) { FireProjectile(); }
    }
    
    // Ship sprite points itself to the mouse cursor's position.
    void PointToMouse()
    {
        Vector3 dirVec = Input.mousePosition - mainCam.WorldToScreenPoint(this.transform.position);
        float angle = (Mathf.Atan2(dirVec.y, dirVec.x) * Mathf.Rad2Deg) - 90.0f;
        this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    // Moves the ship in the direction it is currently facing.
    void MoveShip()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            rb2D.AddForce(this.transform.up * baseShipSpeed);
        }
    }

    // Fires a projectile in the direction the ship is currently facing.
    void FireProjectile()
    {
        if (!isFiringDelayed && Input.GetMouseButton(0))
        {
            Instantiate(projectile, cannon);
            StartCoroutine(DoFiringDelay());
        }
    }

    // Helper function for making a firing delay
    IEnumerator DoFiringDelay()
    {
        isFiringDelayed = true;
        yield return new WaitForSeconds(baseFiringDelay);
        isFiringDelayed = false;
    }

    // Handle collision cases here
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.tag == "Ally")
        {
            int openSlot = findOpenAllySlot();

            if (openSlot != -1)
            {
                BaseAllyScript ally = col.gameObject.GetComponent<BaseAllyScript>();
                Transform chosenSlot = formationSlots[openSlot];

                ally.AttachToPlayer(chosenSlot);
            }
        }
    }

    // Helper function -- finds the first available ally slot
    int findOpenAllySlot()
    {
        for (int i = 0; i < formationSlots.Length; ++i)
        {
            if (formationSlots[i].childCount == 0)
            {
                return i;
            }
        }
        return -1;
    }
}
