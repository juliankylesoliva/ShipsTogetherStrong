using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointEventLogUIScript : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (this.transform.childCount > 5)
        {
            GameObject.Destroy(this.transform.GetChild(0).gameObject);
        }
    }
}
