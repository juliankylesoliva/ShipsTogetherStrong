using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombAllyScript : BaseAllyScript
{
    /* BOMB SHIP VARIABLES */
    public GameObject blastPrefab;

    // Modified to leave an explosion while being destroyed
    public override void DestroyAllyShip(bool isProjectile = false)
    {
        Instantiate(blastPrefab, this.transform.position, Quaternion.identity);
        GameObject.Destroy(this.gameObject);
    }

    // Modified
    public override void DetachFromShip(float ejectSpeed = 0.0f, bool isFromDamage = false)
    {
        if (isFromDamage)
        {
            // Does not explode when the main ship is damaged

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
        else
        {
            // Explodes on the spot when manually ejected
            Instantiate(blastPrefab, this.transform.position, Quaternion.identity);
            GameObject.Destroy(this.gameObject);
        }
    }
}
