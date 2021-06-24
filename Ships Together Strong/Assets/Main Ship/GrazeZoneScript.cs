using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrazeZoneScript : MonoBehaviour
{
    /* PUBLIC VARIABLES */
    public int baseGrazePoints = 1;
    public float grazePointRate = 1.0f;

    public int baseGrazeTime = 1;
    public float grazeTimeMultiplier = 2.0f;

    /* PRIVATE VARIABLES */
    int numProjectiles = 0;

    /* COMPONENTS */
    private Collider2D grazeTrigger;

    /* DRAG AND DROP */
    public Scorekeeper score;
    public MainShipMovement player;
    public SpriteRenderer grazeVisual;

    // Start is called before the first frame update
    void Start()
    {
        grazeTrigger = this.gameObject.GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (numProjectiles > 0)
        {
            grazeVisual.color = new Vector4(1.0f, 0.6677341f, 0.1641509f, 0.1294118f);
        }
        else
        {
            grazeVisual.color = new Vector4(1.0f, 0.6677341f, 0.1641509f, 0.0f);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "EnemyAttack")
        {
            ++numProjectiles;
            StartCoroutine(GrazeTimer(col));
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "EnemyAttack")
        {
            --numProjectiles;
        }
    }

    IEnumerator GrazeTimer(Collider2D targetProjectile)
    {
        if (player.getIsDamaged() || player.getIsDamageproof()) { yield break; } // Do not allow new graze coroutines while damaged

        float rawGrazeScore = 0.0f;
        float grazeTime = 0.0f;

        while (targetProjectile != null && grazeTrigger.IsTouching(targetProjectile)) {
            rawGrazeScore += (grazePointRate * Time.deltaTime);
            grazeTime += Time.deltaTime;
            yield return new WaitForSeconds(0.0f);
        }

        if (player.getIsDamaged() || player.getIsDamageproof()) { yield break; } // Cancel scoring if player is damaged

        score.AddToScore(baseGrazePoints + (int)rawGrazeScore, (baseGrazeTime + (int)(grazeTime * grazeTimeMultiplier)) * (1 + player.getAllyCount()), $"Graze ({grazeTime.ToString("n2")}s)");
    }
}
