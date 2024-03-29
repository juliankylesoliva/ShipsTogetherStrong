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

        PlaySoundEffect(allySounds.soundEffects[0]);

        if (!allyAnim.GetCurrentAnimatorStateInfo(0).IsName("StaticFreefall"))
        {
            allyAnim.Play("StaticFreefall");
        }

        attachHelper(slot);

        attachedTo = AttachType.Player;
        StartCoroutine(DoLifeTimer());
    }

    IEnumerator DoLifeTimer()
    {
        yield return new WaitForSeconds(timeUntilExtraLife);
        MainShipMovement playerShip = GameObject.Find("Main Ship").GetComponent<MainShipMovement>();
        playerShip.incrementLivesLeft();

        PlaySoundEffect(allySounds.soundEffects[7]);
        yield return new WaitForSeconds(1.0f);

        GameObject.Destroy(this.gameObject);
    }

    // Modified
    public override void DetachFromShip(float ejectSpeed = 0.0f, bool isFromDamage = false)
    {
        if (attachedTo == AttachType.Player)
        {
            // Lost if ejected
            Explode();
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

            detachHelper();

            if (ejectSpeed != 0.0f)
            {
                rb2D.AddForce(ejectDirection * ejectSpeed, ForceMode2D.Impulse);
            }

            StartCoroutine(FreefallTimer(baseDespawnTimer));
        }
    }
}
