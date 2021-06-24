using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreFeedUIScript : MonoBehaviour
{
    /* PUBLIC VARIABLES */
    public TMP_Text feedText;
    public float expireTime = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitMessage(Color msgColor, string msg = "Misc")
    {
        feedText.color = msgColor;
        feedText.SetText(msg);
        StartCoroutine(ExpireTimer(expireTime));
    }

    IEnumerator ExpireTimer(float time = 1.0f)
    {
        yield return new WaitForSeconds(time);
        GameObject.Destroy(this.gameObject);
    }

    public void RemoveMessage()
    {
        GameObject.Destroy(this.gameObject);
    }
}
