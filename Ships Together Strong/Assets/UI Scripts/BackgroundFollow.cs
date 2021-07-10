using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundFollow : MonoBehaviour
{
    public Transform playerPosition;
    public float updateRadius = 15.0f;

    void Update()
    {
        Vector3 distanceVec = playerPosition.position - this.transform.position;

        if (distanceVec.x > updateRadius)
        {
            this.transform.position += (Vector3.right * 25.0f);
        }
        else if (distanceVec.x < -updateRadius)
        {
            this.transform.position += (Vector3.left * 25.0f);
        }
        else { }

        if (distanceVec.y > updateRadius)
        {
            this.transform.position += (Vector3.up * 25.0f);
        }
        else if (distanceVec.y < -updateRadius)
        {
            this.transform.position += (Vector3.down * 25.0f);
        }
        else { }
    }
}
