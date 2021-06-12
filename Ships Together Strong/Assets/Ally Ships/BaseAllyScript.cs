using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAllyScript : MonoBehaviour
{
    /* COMPONENTS */
    private Rigidbody2D rb2D;

    // Start is called before the first frame update
    void Start()
    {
        rb2D = this.gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AttachToPlayer(Transform slot)
    {
        rb2D.isKinematic = true;
        rb2D.constraints = RigidbodyConstraints2D.FreezeAll;

        this.transform.parent = slot;
        this.transform.position = slot.position;
        this.transform.rotation = slot.rotation;
    }

    public void DetachFromPlayer()
    {
        this.transform.parent = null;
        rb2D.isKinematic = false;
        rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
