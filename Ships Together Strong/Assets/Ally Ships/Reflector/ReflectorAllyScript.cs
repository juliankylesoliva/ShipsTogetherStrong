using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectorAllyScript : BaseAllyScript
{
    /* REFLECTOR SHIP VARIABLES */
    private int reflectState = 0; // 0 = Cooldown; 1 = Active; 2 = Charged
    public GameObject reflectShotPrefab;
    public Transform cannon;
    public float absorbCooldown = 3.0f;

    /* REFLECTOR SHIP METHODS */

    // Getter method for reflect state
    public int getReflectState()
    {
        return reflectState;
    }

    // Modified
    public override void DestroyAllyShip(bool isProjectile = false)
    {
        if (attachedTo == AttachType.Player)
        {
            if (isProjectile && (reflectState > 0))
            {
                PlaySoundEffect(allySounds.soundEffects[1]);
                spriteRender.sprite = allySprites.spriteList[13];
                reflectState = 2;
            }
            else
            {
                Explode();
                GameObject.Destroy(this.gameObject);
            }
        }
    }

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
        StartCoroutine(DoAbsorbCooldown());
    }

    // Update is called once per frame
    void Update()
    {
        if (attachedTo == AttachType.Player)
        {
            if (isAttached && Input.GetMouseButton(0) && reflectState == 2)
            {
                spriteRender.sprite = allySprites.spriteList[6];
                PlaySoundEffect(allySounds.soundEffects[2]);
                Instantiate(reflectShotPrefab, cannon);
                StartCoroutine(DoAbsorbCooldown());
            }
        }
        else
        {
            if (reflectState != 0)
            {
                reflectState = 0;
                spriteRender.sprite = allySprites.spriteList[6];
            }
        }
    }

    IEnumerator DoAbsorbCooldown()
    {
        reflectState = 0;

        yield return new WaitForSeconds(absorbCooldown);

        if (attachedTo == AttachType.Player)
        {
            spriteRender.sprite = allySprites.spriteList[12];
            reflectState = 1;
        }
    }
}
