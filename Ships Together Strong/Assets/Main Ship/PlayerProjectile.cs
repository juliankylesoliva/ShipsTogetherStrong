using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    /* PROJECTILE VARIABLES */
    public float projectileSpeed = 10.0f;
    public float activeTime = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        transform.parent = null;
        StartCoroutine(expireTimer());
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position += (this.transform.up * Time.deltaTime * projectileSpeed);
    }

    // Projectile despawns after a short period of time
    IEnumerator expireTimer()
    {
        yield return new WaitForSeconds(activeTime);
        GameObject.Destroy(this.gameObject);
    }

    // Checks for collisions
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.tag == "Enemy")
        {
            GameObject.Destroy(col.transform.gameObject);
            GameObject.Destroy(this.gameObject);
        }
        else if (col.transform.tag == "Ally" && (col.transform.parent == null || col.transform.parent.parent.tag == "Enemy"))
        {
            GameObject.Destroy(col.transform.gameObject);
            GameObject.Destroy(this.gameObject);
        }
        else { }
    }
}
