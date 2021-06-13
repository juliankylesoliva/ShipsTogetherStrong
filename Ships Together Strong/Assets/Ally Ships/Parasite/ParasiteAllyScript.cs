using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParasiteAllyScript : BaseAllyScript
{
    /* PARASITE ALLY VARIABLES */
    public float timeUntilLifeLost = 10.0f;
    public float chaseSpeed = 1.5f;
    private GameObject playerShip;

    // Start is called before the first frame update
    void Start()
    {
        rb2D = this.gameObject.GetComponent<Rigidbody2D>();
        soundPlayer = this.gameObject.GetComponent<AudioSource>();
        playerShip = GameObject.Find("Main Ship");
        if (spawnInFreefall) { SpawnInFreefall(); }
    }

    // Modified
    public override void AttachToPlayer(Transform slot)
    {
        if (isAttached) { return; }

        PlaySoundEffect(allySounds.soundEffects[0]);

        rb2D.isKinematic = true;
        rb2D.constraints = RigidbodyConstraints2D.FreezeAll;

        this.transform.parent = slot;
        this.transform.position = slot.position;
        this.transform.rotation = slot.rotation;

        isAttached = true;
        attachedTo = AttachType.Player;
        StartCoroutine(DoLifeTimer());
    }

    IEnumerator DoLifeTimer()
    {
        yield return new WaitForSeconds(timeUntilLifeLost);
        if (playerShip == null)
        {
            playerShip = GameObject.Find("Main Ship");
        }
        MainShipMovement shipTemp = playerShip.GetComponent<MainShipMovement>();
        shipTemp.decrementLivesLeft();

        PlaySoundEffect(allySounds.soundEffects[6]);

        yield return new WaitForSeconds(1.0f);

        Explode();
        GameObject.Destroy(this.gameObject);
    }

    // Modified
    public override void DetachFromShip(float ejectSpeed = 0.0f, bool isFromDamage = false)
    {
        if (attachedTo == AttachType.Player && !isFromDamage)
        {
            // Cannot be ejected
            return;
        }
        else if (attachedTo == AttachType.Player && isFromDamage)
        {
            // Taking damage destroys the parasite
            attachedTo = AttachType.None;
            Explode();
            GameObject.Destroy(this.gameObject);
        }
        else
        {
            // Chases the player without despawning
            this.transform.parent = null;
            rb2D.isKinematic = false;
            rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;

            isAttached = false;

            attachedTo = AttachType.None;

            StartCoroutine(StalkPlayer());
        }
    }

    IEnumerator StalkPlayer()
    {
        if (rb2D == null)
        {
            rb2D = this.gameObject.GetComponent<Rigidbody2D>();
        }

        if (playerShip == null)
        {
            playerShip = GameObject.Find("Main Ship");
        }

        while (attachedTo == AttachType.None)
        {
            pointTowardsPlayer();
            rb2D.AddForce(this.transform.up * chaseSpeed);
            yield return new WaitForSeconds(0.01f);
        }
    }

    // Points toward the player
    void pointTowardsPlayer(float randomizer = 0.0f)
    {
        if (playerShip == null)
        {
            playerShip = GameObject.Find("Main Ship");
        }

        Vector3 dirVec = playerShip.transform.position - this.transform.position;
        float angle = (Mathf.Atan2(dirVec.y, dirVec.x) * Mathf.Rad2Deg) - 90.0f;
        this.transform.rotation = Quaternion.AngleAxis(angle + (Random.Range(-randomizer, randomizer)), Vector3.forward);
    }
}
