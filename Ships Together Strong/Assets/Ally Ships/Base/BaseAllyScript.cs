using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AllyType { Base, Speed, Rapid, Magnify, Shield, Reflector, Copycat, Score, Bomb, Parasite, Life }

public class BaseAllyScript : MonoBehaviour
{
    /* COMPONENTS */
    [HideInInspector] public Rigidbody2D rb2D;

    /* PRIVATE VARIABLES */
    [HideInInspector] public bool isAttached = false;
    [HideInInspector] public float fps = 1.0f / 60.0f;
    [HideInInspector] public float tempTimerConstant = 1.5f;
    [SerializeField] private AllyType powerupType = AllyType.Base; // Can be edited in the inspector

    /* ALLY SHIP VARIABLES */
    public bool spawnInFreefall = false;
    public float baseDespawnTimer = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        rb2D = this.gameObject.GetComponent<Rigidbody2D>();
        if (spawnInFreefall) { SpawnInFreefall(); }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Accessor method for isAttached
    public bool getIsAttached()
    {
        return isAttached;
    }

    // Accessor method for powerupType
    public AllyType getPowerupType()
    {
        return powerupType;
    }

    // Setter method for powerupType
    public void setPowerupType(AllyType type)
    {
        powerupType = type;
    }

    // Spawns the ally ship in a freefall state
    public void SpawnInFreefall()
    {
        DetachFromShip();
    }

    // Attaches ally to player ship
    public void AttachToPlayer(Transform slot)
    {
        if (isAttached) { return; }

        rb2D.isKinematic = true;
        rb2D.constraints = RigidbodyConstraints2D.FreezeAll;

        this.transform.parent = slot;
        this.transform.position = slot.position;
        this.transform.rotation = slot.rotation;

        isAttached = true;
    }

    // Attaches ally to enemy ship
    public void AttachToEnemy(Transform spot)
    {
        if (isAttached) { return; }

        if (rb2D == null)
        {
            rb2D = this.gameObject.GetComponent<Rigidbody2D>();
        }

        rb2D.isKinematic = true;
        rb2D.constraints = RigidbodyConstraints2D.FreezeAll;

        this.transform.parent = spot;
        this.transform.position = spot.position;
        this.transform.rotation = spot.rotation;

        isAttached = true;
    }

    // Detaches ally from its parent
    public virtual void DetachFromShip(float ejectSpeed = 0.0f, bool isFromDamage = false)
    {
        Vector2 ejectDirection;

        if (this.transform.parent != null)
        {
            Vector3 dirVec = (this.transform.position - this.transform.parent.parent.position);
            ejectDirection = new Vector2(dirVec.x, dirVec.y);
        }
        else
        {
            ejectDirection = Vector2.zero;
        }
        

        this.transform.parent = null;
        rb2D.isKinematic = false;
        rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;

        isAttached = false;

        if (ejectSpeed != 0.0f)
        {
            rb2D.AddForce(ejectDirection * ejectSpeed, ForceMode2D.Impulse);
        }

        StartCoroutine(FreefallTimer(baseDespawnTimer));
    }

    // Helper function used to keep track of despawn time
    public IEnumerator FreefallTimer(float seconds)
    {
        if (isAttached) { yield break; }

        float degreesPerSecond = 360.0f / seconds;
        float degreesPerFrame = degreesPerSecond * fps;
        float currDegrees = 0.0f;

        while (currDegrees >= -360.0f && !isAttached)
        {
            currDegrees -= (degreesPerFrame * tempTimerConstant);
            this.transform.rotation = Quaternion.AngleAxis(currDegrees, Vector3.forward);
            yield return new WaitForSeconds(fps);
        }

        if (!isAttached) { GameObject.Destroy(this.gameObject); }
    }

    // Call this to destroy the ally ship
    public virtual void DestroyAllyShip(bool isProjectile = false)
    {
        GameObject.Destroy(this.gameObject);
    }
}
