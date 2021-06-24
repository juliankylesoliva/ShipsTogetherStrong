using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scorekeeper : MonoBehaviour
{
    [HideInInspector] public int totalScore = 0;
    [HideInInspector] public int increasedBy = 0;
    [HideInInspector] public string scoreMessage = "Misc";

    public GameObject scorefeedPrefab;
    public Transform pointEventLog;

    void Start()
    {
        totalScore = 0;
        increasedBy = 0;
        scoreMessage = "Misc";
    }

    void Update()
    {
        //Debug.Log($"Score: {totalScore} ({increasedBy})");
    }

    public void AddToScore(int baseValue = 10, int multiplier = 1, string message = "Misc")
    {
        increasedBy = (baseValue * multiplier);
        totalScore += increasedBy;
        scoreMessage = message;

        GameObject objTemp = Instantiate(scorefeedPrefab, pointEventLog);
        ScoreFeedUIScript feedTemp = objTemp.GetComponent<ScoreFeedUIScript>();

        if (increasedBy > 0)
        {
            feedTemp.InitMessage(new Color(0.0f, 1.0f, 0.0f, 0.5f), $"{scoreMessage}: +{increasedBy} pts");
        }
        else
        {
            feedTemp.InitMessage(new Color(1.0f, 0.0f, 0.0f, 0.5f), $"{scoreMessage}: {increasedBy} pts");
        }
    }
}
