using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scorekeeper : MonoBehaviour
{
    [HideInInspector] public int totalScore = 0;
    [HideInInspector] public int increasedBy = 0;

    void Start()
    {

    }

    void Update()
    {
        //Debug.Log($"Score: {totalScore} ({increasedBy})");
    }

    public void AddToScore(int baseValue = 10, int multiplier = 1)
    {
        increasedBy = (baseValue * multiplier);
        totalScore += increasedBy;
    }
}
