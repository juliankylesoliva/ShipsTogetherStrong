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

    IEnumerator expireTimer()
    {
        yield return new WaitForSeconds(activeTime);
        GameObject.Destroy(this.gameObject);
    }
}
