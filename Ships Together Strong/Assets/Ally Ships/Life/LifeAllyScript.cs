using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeAllyScript : BaseAllyScript
{
    /* LIFE ALLY VARIABLES */
    public float timeUntilExtraLife = 10.0f;

    // Modified
    public override void AttachToPlayer(Transform slot)
    {
        if (isAttached) { return; }

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
        yield return new WaitForSeconds(timeUntilExtraLife);
        MainShipMovement playerShip = GameObject.Find("Main Ship").GetComponent<MainShipMovement>();
        playerShip.incrementLivesLeft();
        GameObject.Destroy(this.gameObject);
    }

    // Modified
    public override void DetachFromShip(float ejectSpeed = 0.0f, bool isFromDamage = false)
    {
        if (attachedTo == AttachType.Player)
        {
            // Lost if ejected
            GameObject.Destroy(this.gameObject);
        }
        else
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

            attachedTo = AttachType.None;

            StartCoroutine(FreefallTimer(baseDespawnTimer));
        }
    }
}
