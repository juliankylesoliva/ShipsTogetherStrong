using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldAllyScript : BaseAllyScript
{
    /* SHIELD SHIP VARIABLES */
    private int numTimesHit = 0;
    public float altDespawnTimer = 1.0f;

    // Modified to take one hit before being destroyed
    public override void DestroyAllyShip()
    {
        ++numTimesHit;
        if (numTimesHit >= 2)
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    // Modified
    public override void DetachFromShip(float ejectSpeed = 0.0f, bool isFromDamage = false)
    {
        // Stays with player if the player is damaged
        if (isFromDamage && numTimesHit == 0)
        {
            ++numTimesHit;
            return;
        }

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
            if (numTimesHit >= 1)
            {
                ejectSpeed *= 2;
            }
            rb2D.AddForce(ejectDirection * ejectSpeed, ForceMode2D.Impulse);
        }

        if (numTimesHit >= 1)
        {
            StartCoroutine(FreefallTimer(altDespawnTimer));
        }
        else
        {
            StartCoroutine(FreefallTimer(baseDespawnTimer));
        }
    }
}
