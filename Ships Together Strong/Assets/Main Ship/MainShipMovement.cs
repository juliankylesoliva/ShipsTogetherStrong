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
    public bool enableEjectMode = true;

    /* SHIP VARIABLES */
    public float baseShipSpeed = 1.0f;
    public float baseFiringDelay = 0.5f;
    public float manualEjectSpeed = 2.0f;
    public float damageEjectSpeed = 5.0f;
    public int startingLives = 3;

    /* POWER-UP MODIFIERS */
    private float modShipSpeed = 1.0f;
    private float modFiringDelay = 1.0f;
    private float modProjectileSize = 1.0f;

    /* PRIVATE SHIP VARIABLES */
    private bool isFiringDelayed = false;
    private bool isEjectModeOn = false;
    private bool isDamaged = false;
    private int totalEnemiesDestroyed = 0;
    private int livesLeft;

    /* PREFABS AND OTHER DRAG AND DROPS */
    public Transform cannon;
    public GameObject projectile;
    public Transform[] formationSlots;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        mainCam = Camera.main;
        rb2D = this.gameObject.GetComponent<Rigidbody2D>();
        livesLeft = startingLives;
    }

    // Update is called once per frame
    void Update()
    {
        if (enableMouseMovement) { PointToMouse(); }
        if (enableShipMovement) { MoveShip(); }
        if (enableShooting) { FireProjectile(); }
        if (enableEjectMode) { EjectModeHandler(); }
    }
    
    // Ship sprite points itself to the mouse cursor's position.
    void PointToMouse()
    {
        if (!isDamaged)
        {
            Vector3 dirVec = Input.mousePosition - mainCam.WorldToScreenPoint(this.transform.position);
            float angle = (Mathf.Atan2(dirVec.y, dirVec.x) * Mathf.Rad2Deg) - 90.0f;
            this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    // Moves the ship in the direction it is currently facing.
    void MoveShip()
    {
        if (!isDamaged && Input.GetKey(KeyCode.Space))
        {
            rb2D.AddForce(this.transform.up * baseShipSpeed * modShipSpeed);
        }
    }

    // Fires a projectile in the direction the ship is currently facing.
    void FireProjectile()
    {
        if (!isDamaged && !isFiringDelayed && Input.GetMouseButton(0))
        {
            GameObject tempObj = Instantiate(projectile, cannon);
            tempObj.transform.localScale *= modProjectileSize;
            StartCoroutine(DoFiringDelay());
        }
    }

    // Helper function for making a firing delay
    IEnumerator DoFiringDelay()
    {
        isFiringDelayed = true;
        yield return new WaitForSeconds(baseFiringDelay * modFiringDelay);
        isFiringDelayed = false;
    }

    // Handle collision cases here
    void OnCollisionEnter2D(Collision2D col)
    {
        if (!isDamaged && col.transform.tag == "Ally")
        {
            int openSlot = findOpenAllySlot();

            if (openSlot != -1)
            {
                BaseAllyScript ally = col.gameObject.GetComponent<BaseAllyScript>();
                Transform chosenSlot = formationSlots[openSlot];

                if (!ally.getIsAttached())
                {
                    ally.AttachToPlayer(chosenSlot);
                    CheckPowerups();
                }
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

    // Function for toggling Eject Mode
    void EjectModeHandler()
    {
        if (isDamaged) { return; }

        if (Input.GetKeyDown(KeyCode.E) && !isEjectModeOn)
        {
            isEjectModeOn = true;
        }

        if (!enableEjectMode || (Input.GetKeyDown(KeyCode.Q) && isEjectModeOn))
        {
            isEjectModeOn = false;
        }

        if (isEjectModeOn)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (isSlotOccupied(0))
                {
                    BaseAllyScript ally = formationSlots[0].GetChild(0).gameObject.GetComponent<BaseAllyScript>();
                    ally.DetachFromShip(manualEjectSpeed);
                    CheckPowerups();
                }
                isEjectModeOn = false;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (isSlotOccupied(1))
                {
                    BaseAllyScript ally = formationSlots[1].GetChild(0).gameObject.GetComponent<BaseAllyScript>();
                    ally.DetachFromShip(manualEjectSpeed);
                    CheckPowerups();
                }
                isEjectModeOn = false;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (isSlotOccupied(2))
                {
                    BaseAllyScript ally = formationSlots[2].GetChild(0).gameObject.GetComponent<BaseAllyScript>();
                    ally.DetachFromShip(manualEjectSpeed);
                    CheckPowerups();
                }
                isEjectModeOn = false;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                if (isSlotOccupied(3))
                {
                    BaseAllyScript ally = formationSlots[3].GetChild(0).gameObject.GetComponent<BaseAllyScript>();
                    ally.DetachFromShip(manualEjectSpeed);
                    CheckPowerups();
                }
                isEjectModeOn = false;
            }
            else { }
        }
    }

    // Helper function -- checks the specified ally slot if a ship is occupied there
    bool isSlotOccupied(int slot)
    {
        return formationSlots[slot].childCount == 1;
    }

    // Updates the ship's stats based on the types of ally ships collected
    void CheckPowerups()
    {
        int speedCount = 0;
        int rapidCount = 0;
        int magnifyCount = 0;

        for (int i = 0; i < formationSlots.Length; ++i)
        {
            if (formationSlots[i].childCount == 1)
            {
                BaseAllyScript ally = formationSlots[i].GetChild(0).gameObject.GetComponent<BaseAllyScript>();
                AllyType type = ally.getPowerupType();

                switch (type)
                {
                    case AllyType.Speed:
                        ++speedCount;
                        break;
                    case AllyType.Rapid:
                        ++rapidCount;
                        break;
                    case AllyType.Magnify:
                        ++magnifyCount;
                        break;
                    default:
                        break;
                }
            }
        }

        modShipSpeed = (1.0f + (speedCount * 0.25f));
        modFiringDelay = (1.0f - (rapidCount * 0.125f));
        modProjectileSize = (1.0f + (magnifyCount * 0.75f));
    }

    // Helper function -- counts how many allies are attached
    int getAllyCount()
    {
        int retVal = 0;

        for (int i = 0; i < formationSlots.Length; ++i)
        {
            if (formationSlots[i].childCount == 1)
            {
                ++retVal;
            }
        }

        return retVal;
    }

    // If the player gets hit while holding ships, all ships get ejected. Otherwise, a life is lost
    public void TakeDamage()
    {
        StartCoroutine(DamageCoroutine());
    }

    private IEnumerator DamageCoroutine()
    {
        if (!isDamaged && getAllyCount() > 0)
        {
            isDamaged = true;

            if (isSlotOccupied(0))
            {
                BaseAllyScript ally = formationSlots[0].GetChild(0).gameObject.GetComponent<BaseAllyScript>();
                ally.DetachFromShip(damageEjectSpeed, true);
            }

            if (isSlotOccupied(1))
            {
                BaseAllyScript ally = formationSlots[1].GetChild(0).gameObject.GetComponent<BaseAllyScript>();
                ally.DetachFromShip(damageEjectSpeed, true);
            }

            if (isSlotOccupied(2))
            {
                BaseAllyScript ally = formationSlots[2].GetChild(0).gameObject.GetComponent<BaseAllyScript>();
                ally.DetachFromShip(damageEjectSpeed, true);
            }

            if (isSlotOccupied(3))
            {
                BaseAllyScript ally = formationSlots[3].GetChild(0).gameObject.GetComponent<BaseAllyScript>();
                ally.DetachFromShip(damageEjectSpeed, true);
            }

            CheckPowerups();

            yield return new WaitForSeconds(1.0f);

            isDamaged = false;
        }
        else
        {
            rb2D.velocity = Vector3.zero;

            isDamaged = true;

            yield return new WaitForSeconds(5.0f);

            isDamaged = false;
        }
    }

    // Accessor method for totalEnemiesDestroyed
    public int getTotalEnemiesDestroyed()
    {
        return totalEnemiesDestroyed;
    }

    // Setter method for totalEnemiesDestroyed
    public void increaseTotalEnemiesDestroyed()
    {
        ++totalEnemiesDestroyed;
    }
}
